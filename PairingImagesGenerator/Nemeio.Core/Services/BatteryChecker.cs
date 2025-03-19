using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    internal class BatteryChecker : BaseKeyboardChecker
    {
        protected override int Timeout => NemeioConstants.NemeioBatteryTimeout;

        public BatteryChecker(IKeyboardComm keyboardComm) : base(keyboardComm) { }

        protected override Task PollTask()
        {
            return KeyboardComm.SendGetBatteryLevel();
        }
    }
}
