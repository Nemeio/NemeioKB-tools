using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Updates
{
    public class NrfInstaller : BaseInstaller
    {
        private const string UPGRADING_FILENAME = "upgrading_nrf";
        private const string UPGRADING_FOLDER_NAME = "upgrading_temp_nrf";

        public NrfInstaller(IDocument documentService) : base(documentService) { }

        public override Task Setup(Keyboard keyboard, Update update)
        {
            //  TODO: Check .dat file exists
            //  TODO: Check .XXX file exists

            //  TODO: Register finish transfer (success / error)
            //  TODO: Start transfer

            throw new NotImplementedException();
        }

        private void UpdateFinished(Exception exception)
        {
            throw new NotImplementedException();
        }

        public override string UnzipFolderName() => UPGRADING_FOLDER_NAME;

        public override string UpgradingFilename() => UPGRADING_FILENAME;
    }
}
