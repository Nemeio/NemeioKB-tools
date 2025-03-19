using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.Exceptions;
using Version = System.Version;

namespace Nemeio.Core.Services.Updates
{
    public enum UpdateStatus
    {
        WaitingUpdate,
        CanDownload,
        Downloading,
        CanApply,
        Applying
    }

    public class UpdateService : IUpdateService
    {
        private const string UpdatesInProgressMessage = "updates_in_progress_message";
        private const string UpdatesApplyFailedMessage = "updates_apply_failed_message";
        private const string UpdatesFinishSuccessMessage = "updates_finish_success_message";
        private const string UpdatesInstallationErrorMessage = "updates_installation_error_message";

        private UpdateStatus _updateStatus;
        private Update _currentUpdate = null;
        private bool _errorOccured = false;
        private readonly IDocument _documentService;
        private readonly ILogger _logger;
        protected readonly List<Update> _updates = new List<Update>();
        private readonly InstallerFactory _installerFactory;
        private WebClient _client;
        private bool _aborted = false;

        internal Action OnUpdateAdded { get; private set; }

        internal Action OnUpdateStarted { get; private set; }

        internal Action OnUpdateEnded { get; private set; }

        internal Action<UpdateStatus> OnUpdateStatusChanged { get; private set; }

        internal Action<string> ShowMessage { get; private set; }

        public UpdateStatus Status
        {
            get => _updateStatus;
            protected set
            {
                _updateStatus = value;

                OnUpdateStatusChanged?.Invoke(_updateStatus);
            }
        }

        public UpdateService(IDocument docSrv, ILoggerFactory loggerFactory)
        {
            _documentService = docSrv;
            _logger = loggerFactory.CreateLogger<IUpdateService>();
            _installerFactory = new InstallerFactory(docSrv);

            Status = UpdateStatus.WaitingUpdate;
        }

        public void SetupOnUpdateAdded(Action added) => OnUpdateAdded = added;

        public void SetupOnUpdateStarted(Action started) => OnUpdateStarted = started;

        public void SetupOnUpdateEnded(Action ended) => OnUpdateEnded = ended;

        public void SetupShowMessage(Action<string> showMsg) => ShowMessage = showMsg;

        public void SetupOnUpdateStatusChanged(Action<UpdateStatus> statusChanged) => OnUpdateStatusChanged = statusChanged;

        public void AddUpdate(Update update)
        {
            Status = UpdateStatus.CanDownload;

            var knowUpdate = _updates.FirstOrDefault(x => x.Type == update.Type);
            if (knowUpdate != null)
            {
                //  We assume that last update is always the newer

                _updates.Remove(knowUpdate);
            }

            _updates.Add(update);

            OnUpdateAdded?.Invoke();
        }

        public async virtual Task DownloadUpdates(Action onDownloadCompleted, Action onErrorRaised)
        {
            Status = UpdateStatus.Downloading;

            CreateTemporaryFolderIfNeeded();

            foreach (var update in _updates)
            {
                if (_errorOccured)
                {
                    OnDowloadingErrorOccurred();

                    onErrorRaised?.Invoke();

                    break;
                }

                update.ComputeInstallerPath(_documentService);

                if (!File.Exists(update.InstallerPath))
                {
                    using (_client = new WebClient())
                    {
                        try
                        {
                            _client.DownloadFileCompleted += client_DownloadFileCompleted;

                            await _client.DownloadFileTaskAsync(
                                update.Url,
                                update.InstallerPath
                            );

                            _errorOccured = !VerifyFile(update.InstallerPath, update.Checksum);
                        }
                        catch (WebException e)
                        {
                            _logger.LogError($"Download error {e.Message}");

                            _errorOccured = true;
                        }
                    }
                }
            }

            if (_errorOccured)
            {
                OnDowloadingErrorOccurred();

                onErrorRaised?.Invoke();

                return;
            }

            Status = UpdateStatus.CanApply;

            onDownloadCompleted?.Invoke();
        }

        void OnDowloadingErrorOccurred()
        {
            Status = UpdateStatus.WaitingUpdate;

            _errorOccured = false;

            CleanTemporaryFolder();
        }

        public async Task ApplyUpdates(Keyboard keyboard)
        {
            if (UpdateInProgress()) { return; }

            Status = UpdateStatus.Applying;

            UpdateStart(keyboard);

            if (_updates.Count <= 0) { UpdateEnd(keyboard); }

            _currentUpdate = GetNextUpdate();
            _currentUpdate.ComputeInstallerPath(_documentService);

            if (_currentUpdate.IsKeyboardUpdate()) { ShowMessage?.Invoke(UpdatesInProgressMessage); }

            var installer = _installerFactory.CreateInstaller(_currentUpdate.Type);

            try
            {
                installer.Unzip(_currentUpdate);

                await installer.Setup(keyboard, _currentUpdate);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is FileNotFoundException || ex is InvalidDataException)
            {
                _logger.LogError($"Apply update error {ex.Message}");

                ApplyUpdateErrorOccured();
            }

            UpdateEnd(keyboard);
        }

        void UpdateStart(Keyboard keyboard)
        {
            keyboard?.StartUpdate();

            OnUpdateStarted?.Invoke();
        }

        void UpdateEnd(Keyboard keyboard)
        {
            keyboard?.EndUpdate();

            OnUpdateEnded?.Invoke();
        }

        Update GetNextUpdate()
        {
            _updates.Sort((up1, up2) => up1.CompareTo(up2));

            return _updates[0];
        }

        void ApplyUpdateErrorOccured()
        {
            Status = UpdateStatus.WaitingUpdate;

            ShowMessage?.Invoke(UpdatesApplyFailedMessage);
        }

        public void CheckAppUpdate(DataModels.VersionProxy appVersionProxy)
        {
            var installer = _installerFactory.CreateInstaller(UpdateType.App);
            var status = installer.VerifyUpdate(appVersionProxy);

            if (status == InstallationStatus.Error)
            {
                ShowMessage?.Invoke(UpdatesApplyFailedMessage);

                CleanTemporaryFolder();
            }
        }

        public async Task CheckKeyboardUpdatesInProgress(Keyboard keyboard)
        {
            if (_currentUpdate == null)
            {
                //  No update in progress, check tmp file
                var stm32Installer = _installerFactory.CreateInstaller(UpdateType.Stm32);
                var nrfInstaller = _installerFactory.CreateInstaller(UpdateType.Nrf);

                if (_currentUpdate == null)
                {
                    _currentUpdate = stm32Installer.LoadTempUpdate();
                }

                if (_currentUpdate == null)
                {
                    _currentUpdate = nrfInstaller.LoadTempUpdate();
                }

                if (_currentUpdate == null)
                {
                    return;
                }
            }

            await keyboard.RequestVersions();

            var installer = _installerFactory.CreateInstaller(_currentUpdate.Type);
            VersionProxy version = null;

            switch (_currentUpdate.Type)
            {
                case UpdateType.Nrf:
                    version = keyboard.NrfVersionProxy;
                    break;
                case UpdateType.Stm32:
                    version = keyboard.Stm32VersionProxy;
                    break;
                default:
                    throw new InvalidUpdateTypeException();
            }

            var updateStatus = installer.VerifyUpdate(version);

            if (updateStatus == InstallationStatus.Success)
            {
                var updatePath = Path.Combine(
                    _documentService.TemporaryFolderPath,
                    Path.GetFileName(_currentUpdate.Url.ToString())
                );

                try
                {
                    File.Delete(updatePath);

                    EndCurrentUpdate();

                    if (_updates.Count > 0)
                    {
                        //  Update is successful and other update is pending,
                        //  we assume that user want to continue update

                        await ApplyUpdates(keyboard);
                    }
                    else
                    {
                        //  Every update is finished

                        Status = UpdateStatus.WaitingUpdate;

                        CleanTemporaryFolder();

                        ShowMessage?.Invoke(UpdatesFinishSuccessMessage);
                    }
                }
                catch (Exception e) when (e is ArgumentException || e is DirectoryNotFoundException || e is IOException || e is UnauthorizedAccessException)
                {
                    EndCurrentUpdate();

                    InstallationFailed();
                }
            }
            else if (updateStatus == InstallationStatus.Error)
            {
                InstallationFailed();

                CleanTemporaryFolder();
            }
        }
        
        private void InstallationFailed()
        {
            Status = UpdateStatus.WaitingUpdate;

            ShowMessage?.Invoke(UpdatesInstallationErrorMessage);
        }

        private void EndCurrentUpdate()
        {
            _updates.Remove(_currentUpdate);
            _currentUpdate = null;
        }

        public void FirmwareUpdateFailed(int errorCode)
        {
            //  Updating but an error occurred ...

            _currentUpdate = null;

            Status = UpdateStatus.WaitingUpdate;

            CleanTemporaryFolder();

            ShowMessage?.Invoke(UpdatesApplyFailedMessage);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _errorOccured = e.Error != null;

            if (_aborted)
            {
                CleanTemporaryFolder();
            }
        }

        public bool VerifyFile(string filePath, string checksum) => CalculateMD5(filePath) == checksum;

        void CreateTemporaryFolderIfNeeded() => Directory.CreateDirectory(_documentService.TemporaryFolderPath);

        void CleanTemporaryFolder() => Directory.Delete(_documentService.TemporaryFolderPath, true);

        string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        bool UpdateInProgress() => _currentUpdate != null;

        public void Abort()
        {
            _aborted = true;
            _client?.CancelAsync();

            CleanTemporaryFolder();
        }
    }
}
