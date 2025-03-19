using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class FormatFlash : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID FormatFlashCmdId = InstallToolDefs.AppCommCmdID.FORMATFLASH;

        public FormatFlash(AppComm appComm) : base(appComm, FormatFlashCmdId)
        {
        }

        public void format()
        {
            byte[] emptyBuff = new byte[0];

            mAppComm.Send(FormatFlashCmdId, emptyBuff);
        }
    }
}
