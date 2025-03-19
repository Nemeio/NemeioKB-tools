using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nemeio.Core.Controllers;

namespace Nemeio.Core.Services
{
    class ApplicationUpdateChecker : BaseChecker
    {
        private readonly IKeyboardController _keyboardCtrl;

        protected override int Timeout => NemeioConstants.NemeioUpdateVersionTimeout;

        public ApplicationUpdateChecker(IKeyboardController ctrl) : base() => _keyboardCtrl = ctrl;

        protected override Task PollTask() => _keyboardCtrl.CheckApplicationUpdate();
    }
}
