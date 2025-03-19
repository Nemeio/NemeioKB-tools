using System.Threading;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    internal class KeepAlive : BaseKeyboardChecker
    {
        protected override int Timeout => NemeioConstants.NemeioKeepAliveTimeout;

        public KeepAlive(IKeyboardComm keyboardComm) : base(keyboardComm) { }

        protected override Task PollTask() => KeyboardComm.KeepAlive();
    }
}
