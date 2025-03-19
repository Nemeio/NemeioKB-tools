using Nemeio.Core.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core.Services.Updates
{
    public class StmInstaller : BaseInstaller
    {
        private const string UPGRADING_FILENAME = "upgrading_stm32";
        private const string UPGRADING_FOLDER_NAME = "upgrading_temp_stm";

        public StmInstaller(IDocument documentService) : base(documentService) { }

        public override async Task Setup(Keyboard keyboard, Update update)
        {
            var installerPath = UnzipFolderPath();

            if (!Directory.Exists(installerPath))
            {
                throw new FileNotFoundException("Can't install stm32 update", installerPath);
            }

            var installerFile = Directory.EnumerateFiles(installerPath).First();

            SaveTempUpdate(update);

            await keyboard.SendUpdateFile(
                File.ReadAllBytes(installerFile)
            );
        }

        public override string UnzipFolderName() => UPGRADING_FOLDER_NAME;

        public override string UpgradingFilename() => UPGRADING_FILENAME;
    }
}
