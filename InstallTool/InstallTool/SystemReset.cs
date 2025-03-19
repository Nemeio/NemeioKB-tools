using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class SystemReset : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID SystemResetCmdId = InstallToolDefs.AppCommCmdID.SYSTEMRESET;

        public SystemReset(AppComm appComm) : base(appComm, SystemResetCmdId)
        {

        }

        public bool reset()
        {
            byte[] emptyBuff = new byte[0];
            
            mAppComm.Send(SystemResetCmdId, emptyBuff);

            return true;
        }
    }
}
