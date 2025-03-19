using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nemeio.Core.Extensions;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services;
using System.Threading;
using Nemeio.Core.Managers;
using Nemeio.Core.DataModels;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Controllers
{
    public class KeyboardController : IKeyboardController
    {
        private readonly IOsKeyboardProxy _osKeyboardProxy;
        private readonly IBrowserFile _appService;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;
        private readonly IKbInfoDisplayer _keyboardInformationDisplayer;
        private readonly KeystrokeInterpreter _keystrokeInterpreter = new KeystrokeInterpreter();
        private readonly INemeioHttpService _nemeioHttpService;
        private readonly IInformationService _infoService;
        private Keyboard _keyboard;
        private readonly ApplicationUpdateChecker _applicationUpdateChecker;
        private readonly IUpdateService _updateService;

        public KeyboardController(ILoggerFactory loggerFactory, INemeioHttpService nemeioHttpService, IDialogService dialog, IBrowserFile app,
            IOsKeyboardProxy keyboardProxy, IKbInfoDisplayer keyboardInformationDisplayer, IOsSessionLockWatcher osSessionLockWatcher, IInformationService informationService, IUpdateService updateService)
        {
            _updateService = updateService;
            _updateService.SetupOnUpdateStarted(UpdateStarted);
            _updateService.SetupOnUpdateEnded(UpdateEnded);
            _updateService.CheckAppUpdate(informationService.GetAppVersion());

            _nemeioHttpService = nemeioHttpService;
            _osKeyboardProxy = keyboardProxy;
            _logger = loggerFactory.CreateLogger<KeyboardController>();
            _dialogService = dialog;
            _appService = app;
            _keyboardInformationDisplayer = keyboardInformationDisplayer;
            _infoService = informationService;
            _applicationUpdateChecker = new ApplicationUpdateChecker(this);
            _osKeyboardProxy.Init(async (com) => await InitKeyboard(com), Disconnect, SwitchLayout);
            osSessionLockWatcher.Setup(SetFallbackCommunication, SetDefaultCommunication);
        }

        public Keyboard GetKeyboard() => _keyboard;

        public async Task InitKeyboard(ComPort comPort)
        {
            if (_keyboard != null) { return; }

            _keyboard = new Keyboard(
                comPort, 
                _osKeyboardProxy, 
                _keyboardInformationDisplayer, 
                _updateService,
                _nemeioHttpService, 
                DigestBatteryLevel,
                KeyPressed
            );

            _keyboardInformationDisplayer.UpdateConnectionState(ConnectionStatus.Connected);

            await _updateService.CheckKeyboardUpdatesInProgress(_keyboard);
            await _keyboard.HandlePotentialCrashInformation();
        }

        public async Task SwitchLayout()
        {
            if (_keyboard == null) { return; }

            await _keyboard.SetLayoutFromOsToKeyboard();
        }

        public async Task RefreshLayout()
        {
            if (_keyboard == null) { return; }

            await _keyboard.RefreshLayoutFromDatabaseToKeyboard();
        }

        public async Task UpdateLayout(LayoutId id)
        {
            if (_keyboard == null) { return; }

            await _keyboard.UpdateLayoutFromDatabase(id);
        }

        public async Task RemoveLayoutFromKeyboard(LayoutId id)
        {
            if (_keyboard == null) { return; }

            await _keyboard.DeleteLayout(id);
        }
        public async Task CheckApplicationUpdate()
        {
            try
            {
                var infos = await _nemeioHttpService.CheckForUpdates();

                var appVersion = _infoService.GetAppVersion();
                var cloudAppUpdate = GetCloudAppUpdate(infos);

                var appNeedUpdate = cloudAppUpdate.VersionProxy.IsHigherThan(appVersion);
                if (appNeedUpdate)
                {
                    _keyboardInformationDisplayer.ShowNotification("update_available_title", "app_update_available_message");
                    _updateService.AddUpdate(cloudAppUpdate);
                }
            }
            catch (HttpRequestException exception)
            {
                //  Nothing to do here
                //  KeyboardUpdateChecker will verify automatically for a new update later
                
                _logger.LogError($"Webserver not responds : {exception.Message}");
            }
        }

        private Update GetCloudAppUpdate(UpdateModel updateModel)
        {
            VersionModel version;

            if (this.IsOSXPlatform())
            {
                version = updateModel.OSX;
            }
            else
            {
                version = updateModel.Windows;
            }

            return new Update(
                version.Url,
                new VersionProxy(new Version(version.Version)),
                UpdateType.App,
                version.Checksum
            );
        }

        public void DisconnectAll()
        {
            _keyboard?.Disconnect();
            _keyboard = null;
        }

        public void Disconnect(ComPort comPort)
        {
            _keyboard?.Disconnect();
            _keyboard = null;
            _keyboardInformationDisplayer.UpdateBatteryLevel(BatteryLevel.NotPlugged);
            _keyboardInformationDisplayer.UpdateConnectionState(ConnectionStatus.Disconnected);
        }

        private void DigestBatteryLevel(BatteryLevel batteryModel)
        {
            if (batteryModel == null) { return; }

            if (batteryModel <= NemeioConstants.NemeioMinimumBatteryLevel)
            {
                _keyboardInformationDisplayer.ShowNotification("battery_notification_title", "battery_notification_message");
            }

            _keyboardInformationDisplayer.UpdateBatteryLevel(batteryModel);
        }

        private void KeyPressed(NemeioIndexKeystroke[] keystrokes)
        {
            _logger.LogInformation("DidReceiveKeystrokes");

            var currentLayout = _keyboard.Layout;
            if (currentLayout == null)
            {
                _logger.LogWarning("No layout found, can't compute keys");

                return;
            }

            var actions = _keystrokeInterpreter.GetActions(currentLayout, keystrokes);
            int receveidKeyStroke = actions.Count;
            if (receveidKeyStroke == 0)
            {
                _osKeyboardProxy.StopSendKey();
                return;
            }

            var utf8s = new List<string>();
            var applications = new List<string>();
            var urls = new List<string>();

            for (int i = 0; i < receveidKeyStroke; i++)
            {
                var action = actions[i];
                var actionData = action.Data;

                switch (action.Type)
                {
                    case KeyActionType.Unicode:
                    case KeyActionType.Special:
                        utf8s.Add(actionData);
                        break;

                    case KeyActionType.Application:
                        applications.Add(actionData);
                        break;

                    case KeyActionType.Url:
                        urls.Add(actionData);
                        break;
                }
            }

            if (utf8s.Count > 0)
            {
                SendSequenceToOs(utf8s.ToArray());
            }

            if (applications.Count > 0)
            {
                try
                {
                    _appService.OpenApplications(applications.ToArray());
                }
                catch (Exception e)
                {
                    _dialogService.Error(e.Message + ": \n" + applications[0]);
                }
            }

            if (urls.Count > 0)
            {
                try
                {
                    _appService.OpenUrls(urls.ToArray());
                }
                catch (Exception e)
                {
                    _dialogService.Error(e.Message);
                }
            }
        }

        private void SendSequenceToOs(string[] keys)
        {
            _logger.LogInformation($"SendSequence {keys.ToReadeableString()} on thread : {Thread.CurrentThread.ManagedThreadId}");
            _osKeyboardProxy.PostHidStringKeys(keys);
        }

        public async Task SetFallbackCommunication()
        {
            if (_keyboard == null) return;
            var curr = _osKeyboardProxy.GetCurrentOsLayout();
            await _keyboard.SetHid(curr.LayoutId);
        }

        public async Task SetDefaultCommunication()
        {
            if (_keyboard == null) return;
            await _keyboard.LeaveHid();
        }

        void UpdateStarted() => _keyboardInformationDisplayer.ShowUpgrading();

        void UpdateEnded()
        {
            var status = _keyboard != null ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;

            _keyboardInformationDisplayer.UpdateConnectionState(status);
        }
    }
}
