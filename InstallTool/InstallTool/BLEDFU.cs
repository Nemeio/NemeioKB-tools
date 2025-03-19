using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class BLEDFU : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID BLEDFUCmdId = InstallToolDefs.AppCommCmdID.BLEDFU;

        public BLEDFU(AppComm appComm) : base(appComm, BLEDFUCmdId)
        {

        }

        public void test()
        {
            byte[] testData = new byte[] { 0x01, 0x02, 0x03, 0xC0 };
            mAppComm.Send(BLEDFUCmdId, testData);
        }
    }
}
