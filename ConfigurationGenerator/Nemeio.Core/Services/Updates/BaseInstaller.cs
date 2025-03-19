using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Updates
{
    public enum InstallationStatus
    {
        Success,
        Error,
        NoUpdate
    }

    public abstract class BaseInstaller
    {
        private IDocument _documentService;

        public BaseInstaller(IDocument documentService)
        {
            _documentService = documentService;
        }

        public abstract Task Setup(Keyboard keyboard, Update update);

        public abstract string UpgradingFilename();

        public abstract string UnzipFolderName();

        public void Unzip(Update update)
        {
            if (update.InstallerPath == null)
            {
                throw  new ArgumentNullException(nameof(update.InstallerPath));
            }

            if (!File.Exists(update.InstallerPath))
            {
                throw new FileNotFoundException("Can't find file", update.InstallerPath);
            }

            var unzipFolderPath = UnzipFolderPath();

            if (!Directory.Exists(unzipFolderPath))
            {
                Directory.CreateDirectory(unzipFolderPath);
            }

            ZipFile.ExtractToDirectory(update.InstallerPath, unzipFolderPath);
        }

        public InstallationStatus VerifyUpdate(VersionProxy appVersionProxy)
        {
            var upgradingFilePath = UpgradingFilePath();

            if (!File.Exists(upgradingFilePath))
            {
                return InstallationStatus.NoUpdate;
            }

            var tempUpdate = LoadTempUpdate();

            File.Delete(upgradingFilePath);

            return tempUpdate.VersionProxy == appVersionProxy ? InstallationStatus.Success : InstallationStatus.Error;
        }

        protected string UpgradingFilePath()
            => Path.Combine(
                UpgradingTempFolderPath(_documentService),
                UpgradingFilename()
            );

        protected string UnzipFolderPath()
            => Path.Combine(
                _documentService.TemporaryFolderPath,
                UnzipFolderName()
            );

        public void SaveTempUpdate(Update update)
        {
            var path = UpgradingFilePath();
            var tempUpdate = TempUpdate.FromDomainModel(update);

            using (var stream = File.Open(path, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, tempUpdate);
            }
        }

        public Update LoadTempUpdate()
        {
            var path = UpgradingFilePath();

            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                using (var stream = File.Open(path, FileMode.Open))
                {
                    var binaryFormatter = new BinaryFormatter();

                    var tempUpdate = (TempUpdate) binaryFormatter.Deserialize(stream);

                    return tempUpdate.ToDomainModel();
                }
            }
            catch (SerializationException exception)
            {
                File.Delete(path);
            }
            catch (InvalidCastException exception)
            {
                File.Delete(path);
            }

            return null;
        }

        protected static string UpgradingTempFolderPath(IDocument documentService) => documentService.TemporaryFolderPath;
    }
}
