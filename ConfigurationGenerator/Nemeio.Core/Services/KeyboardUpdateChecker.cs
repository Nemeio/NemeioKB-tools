using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    internal class KeyboardUpdateChecker : BaseKeyboardChecker
    {
        private Keyboard _keyboard;

        protected override int Timeout => NemeioConstants.NemeioUpdateVersionTimeout;

        public KeyboardUpdateChecker(Keyboard kb, IKeyboardComm keyboardComm) : base(keyboardComm) => _keyboard = kb;

        protected override Task PollTask()
        {
            if (!_keyboard.IsAvailable())
            {
                return Task.Delay(0);
            }

            return KeyboardComm.GetFirmwareVersions();
        }
    }
}
