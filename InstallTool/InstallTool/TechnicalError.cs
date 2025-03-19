using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class TechnicalError : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID TechnicalErrorCmdId = InstallToolDefs.AppCommCmdID.TECHNICALERROR;
        private const int DelayKeepAlive = 1000;

        public TechnicalError(AppComm appComm) : base(appComm, TechnicalErrorCmdId)
        {
        }

        public void test(int durationSec)
        {
            Thread.Sleep(durationSec * 1000);
        }

        private UInt32 parse(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    byte[] errCodeArray = binaryReader.ReadBytes(4);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(errCodeArray);

                    UInt32 errCode = BitConverter.ToUInt32(errCodeArray, 0);                    

                    return errCode;
                }
            }
        }

        public override void CmdReceived(byte[] data)
        {
            if (data.Length != sizeof(UInt32))
            {
                Console.WriteLine("Wrong data");
            }
            else
            {
                UInt32 errCode = parse(data);

                Console.WriteLine("Received technical error code: {0:X8}", errCode);
            }
        }
    }
}
