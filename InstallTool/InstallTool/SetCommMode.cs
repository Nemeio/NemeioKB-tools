using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class SetCommMode : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID SetCommModeCmdId = InstallToolDefs.AppCommCmdID.SETCOMMMODE;
        private enum CommMode : byte
        {
            HID = 1,
            CONNECTED = 2
        }
        private enum ResponseRetCode : sbyte { SUCCESS = 0 };
        private const int DelayKeepAlive = 1000;

        private KeyPressed mKeyPressedCmd;
        private CancellationTokenSource mTokenKeepAliveSource;
        private CancellationTokenSource mTokenGetCommandSource;

        public SetCommMode(AppComm appComm) : base(appComm, SetCommModeCmdId)
        {
            mKeyPressedCmd = new KeyPressed(appComm);
        }

        public void test()
        {
            Console.WriteLine(@"Usage: 
    ESC  : quit the application
    #0-9 : send the SetCommMode command with the corresponding value as argument
            1 : HID mode
            2 : Connected mode");
            mTokenKeepAliveSource = new CancellationTokenSource();
            mTokenGetCommandSource = new CancellationTokenSource();

            Task taskKeepAlive = new Task(() => { threadKeepAlive(); });
            Task taskGetCommand = new Task(() => { threadGetCommand(); });

            taskKeepAlive.Start();
            taskGetCommand.Start();

            taskKeepAlive.Wait();
            taskGetCommand.Wait();
        }

        private void threadKeepAlive()
        {
            KeepAlive keepAlive = new KeepAlive(mAppComm);

            while(!mTokenKeepAliveSource.IsCancellationRequested)
            {
                keepAlive.sendKeepAlive();
                Thread.Sleep(DelayKeepAlive);
            }
        }

        private void threadGetCommand()
        {
            while (!mTokenKeepAliveSource.IsCancellationRequested)
            {
                if(!getSetCommModeCommand())
                {
                    mTokenGetCommandSource.Cancel();
                    mTokenKeepAliveSource.Cancel();
                }

            }
        }

        private bool getSetCommModeCommand()
        {
            bool bContinue = true;

            ConsoleKeyInfo inputInfo = Console.ReadKey();
            if(inputInfo.Key == ConsoleKey.Escape)
            {
                bContinue = false;
            }
            else
            {
                char inputChar = inputInfo.KeyChar;
                if(Char.IsNumber(inputChar))
                {
                    setCommMode((CommMode)Char.GetNumericValue(inputChar));
                }
            }

            return bContinue;
        }

        private bool setCommMode(CommMode mode)
        {
            Console.WriteLine("Set mode {0}", mode);

            byte[] data = new byte[1];
            data[0] = (byte)mode;
            mAppComm.Send(SetCommModeCmdId, data);

            bool bRet = getResponse();

            if (bRet)
            {
                Console.WriteLine("Mode successfully set");
            }

            return bRet;
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
