using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System;

namespace Nemeio.Core.Services
{
    internal abstract class ResourceWatcher<T>
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly Func<Task> _checkResource;
        private readonly Func<Task<T>> _checkResourceWithResult;
        private readonly Action<T> _checkedResourceHandler;
        private System.Timers.Timer _timer;

        private ResourceWatcher()
        {
            _cancellationToken = new CancellationTokenSource();
            _timer = new System.Timers.Timer(NemeioConstants.NemeioBatteryTimeout) { AutoReset = true };
            _timer.Elapsed += CheckHandler;
        }

        protected ResourceWatcher(Func<Task> checkResource) : this()
        {
            _checkResource = checkResource;
        }

        protected ResourceWatcher(Func<Task<T>> checkResource, Action<T> checkedResourceHandler) : this()
        {
            _checkResourceWithResult = checkResource;
            _checkedResourceHandler = checkedResourceHandler;
        }

        protected void Start()
        {
            CheckHandler(null, null);
            _timer.Start();
        }

        private void CheckHandler(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () =>
            {
                if (_checkResource != null)
                {
                    await _checkResource();
                }
                else if (_checkResourceWithResult != null)
                {
                    T res = await _checkResourceWithResult();
                    _checkedResourceHandler(res);
                }
            }, _cancellationToken.Token);
        }

        public void Stop()
        {
            _cancellationToken?.Cancel();
            if (_timer == null) { return; }
            _timer.Stop();
            _timer.Elapsed -= CheckHandler;
            _timer.Dispose();
            _timer = null;

        }
    }
}
