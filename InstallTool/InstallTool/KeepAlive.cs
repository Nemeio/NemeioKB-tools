using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class KeepAlive : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID KeepAliveCmdId = InstallToolDefs.AppCommCmdID.KEEPALIVE;

        public KeepAlive(AppComm appComm) : base(appComm, KeepAliveCmdId)
        {

        }
        
        public void sendKeepAlive()
        {
            sendKeepAliveFrame();
        }

        public void sendKeepAlive(int nbKeepAlive, int delayMs)
        {
            for(int i = 0; i < nbKeepAlive; i++)
            {
                if(!sendKeepAliveFrame())
                {
                    break;
                }

                Thread.Sleep(delayMs);
            }
        }

        private bool sendKeepAliveFrame()
        {
            byte[] emptyBuff = new byte[0];

            Console.WriteLine("Send KeepAlive");
            mAppComm.Send(KeepAliveCmdId, emptyBuff);

            return getResponse();
        }

        public byte[] getFrame()
        {
            byte[] emptyBuff = new byte[0];
            byte[] frameBuff;
            mAppComm.GetFrameBytes(KeepAliveCmdId, emptyBuff, out frameBuff);

            return frameBuff;
        }

        public bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length == 0)
            {
                bRet = true;
            }

            if (bRet)
            {
                Console.WriteLine("Received keep alive");
            }
            else
            {
                Console.WriteLine("Error receiving response");
            }

            return bRet;
        }

    }
}
