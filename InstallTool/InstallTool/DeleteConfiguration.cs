using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class DeleteConfiguration : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID DeleteConfigurationCmdId = InstallToolDefs.AppCommCmdID.DELETECONFIG;
        private enum ResponseRetCode : sbyte { ACK = 0 };

        public DeleteConfiguration(AppComm appComm) : base(appComm, DeleteConfigurationCmdId)
        {

        }

        public void delete(string confName)
        {
            mAppComm.Send(DeleteConfigurationCmdId, Encoding.ASCII.GetBytes(confName));

            if (getResponse())
            {
                Console.WriteLine("Configuration successfully deleted");
            }
        }

        private bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(ResponseRetCode))
            {
                ResponseRetCode respRetCode = (ResponseRetCode)response[0];
                if (respRetCode == ResponseRetCode.ACK)
                {
                    bRet = true;
                }
                else
                {
                    Console.WriteLine("Received error " + respRetCode);
                }
            }
            else
            {
                Console.WriteLine("Error receiving response");
            }

            return bRet;
        }
    }
}
