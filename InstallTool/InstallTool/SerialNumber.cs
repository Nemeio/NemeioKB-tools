using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class SerialNumber : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID SerialNumberCmdId = InstallToolDefs.AppCommCmdID.SERIALNUMBER;
        private const int SerialNumberMaxLength = 12;

        public SerialNumber(AppComm appComm) : base(appComm, SerialNumberCmdId)
        {

        }

        public void get()
        {
            byte[] emptyBuff = new byte[0];

            mAppComm.Send(SerialNumberCmdId, emptyBuff);

            getResponse();
        }

        private bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length == SerialNumberMaxLength)
            {
                Console.Write("Serial Number: ");
                foreach (var num in response)
                {
                    Console.Write("{0:X2} ", num);
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("\r\nError receiving response");
            }

            return bRet;
        }
    }
}
