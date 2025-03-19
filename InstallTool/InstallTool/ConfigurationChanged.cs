using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class ConfigurationChanged : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID ConfigurationChangedCmdId = InstallToolDefs.AppCommCmdID.CONFIGCHANGED;

        public ConfigurationChanged(AppComm appComm) : base(appComm, ConfigurationChangedCmdId)
        {
        }

        public void test(int durationSec)
        {
            Thread.Sleep(durationSec * 1000);
        }

        public override void CmdReceived(byte[] data)
        {
            Console.WriteLine("Configuration changed to " + System.Text.Encoding.Default.GetString(data));
        }
    }
}
