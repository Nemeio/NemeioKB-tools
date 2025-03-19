using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    interface IAppCommCmdListener
    {
        InstallToolDefs.AppCommCmdID CmdId { get; }
        void CmdReceived(byte[] data);
    }
}
