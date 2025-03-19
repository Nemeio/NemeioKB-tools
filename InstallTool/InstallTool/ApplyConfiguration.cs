using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class ApplyConfiguration : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID ApplyConfigurationCmdId = InstallToolDefs.AppCommCmdID.APPLYCONFIG;
        private enum ResponseRetCode : sbyte { ACK = 0 };

        public ApplyConfiguration(AppComm appComm) : base(appComm, ApplyConfigurationCmdId)
        {

        }

        public void apply(string confName)
        {
            mAppComm.Send(ApplyConfigurationCmdId, Encoding.ASCII.GetBytes(confName));

            if(getResponse())
            {
                Console.WriteLine("Configuration successfully applied");
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
