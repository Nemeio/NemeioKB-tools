using Nemeio.Core.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core.Services.Updates
{
    public abstract class AppInstaller : BaseInstaller
    {
        private const string UPGRADING_FILENAME = "upgrading_app";
        private const string UPGRADING_FOLDER_NAME = "upgrading_temp_app";

        public AppInstaller(IDocument documentService) : base(documentService) { }

        public override async Task Setup(Keyboard keyboard, Update update)
        {
            var installerPath = UnzipFolderPath();

            if (!Directory.Exists(installerPath))
            {
                throw new FileNotFoundException("Can't install application update", installerPath);
            }

            var installerFile = Directory.EnumerateFiles(installerPath).First(x => x.EndsWith(InstallerExtensionFilter()));

            try
            {
                if (keyboard != null)
                {
                    await keyboard.SetHid(keyboard.GetDefaultLayout());
                }
            }
            catch (InvalidOperationException e)
            {
                //  Can't set hid mode
            }

            //  Create an empty file to remember that one comes from an update after restart
            SaveTempUpdate(update);

            await Install(installerFile);
        }

        public override string UnzipFolderName() => UPGRADING_FOLDER_NAME;

        public override string UpgradingFilename() => UPGRADING_FILENAME;

        public abstract string InstallerExtensionFilter();

        public abstract Task Install(string filePath);
    }
}
