using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    abstract class BaseChecker
    {
        private readonly CancellationTokenSource _cancellation;

        public BaseChecker()
        {
            _cancellation = new CancellationTokenSource();
            Task.Run(Poll, _cancellation.Token);
        }

        private async Task Poll()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                await PollTask();
                _cancellation.Token.WaitHandle.WaitOne(Timeout);
            }
        }

        protected abstract int Timeout { get; }

        protected abstract Task PollTask();

        public void Stop() => _cancellation.Cancel();
    }
}
