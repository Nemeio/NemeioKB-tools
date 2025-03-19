using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    interface IAppCommRespListener
    {
        InstallToolDefs.AppCommCmdID CmdId { get; }
        void ResponseReceived(byte[] data);
    }
}
