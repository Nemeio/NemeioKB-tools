using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class FactoryReset : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID FactoryResetCmdId = InstallToolDefs.AppCommCmdID.FACTORYRESET;

        private enum ResponseRetCode : sbyte { SUCCESS = 0 };

        public FactoryReset(AppComm appComm) : base(appComm, FactoryResetCmdId)
        {
        }

        public void reset()
        {
            byte[] emptyBuff = new byte[0];

            mAppComm.Send(FactoryResetCmdId, emptyBuff);

            getResponse();
        }

        private bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(ResponseRetCode))
            {
                ResponseRetCode respRetCode = (ResponseRetCode)response[0];
                if (respRetCode == ResponseRetCode.SUCCESS)
                {
                    bRet = true;
                    Console.WriteLine("Device successfully reset to factory settings");
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
