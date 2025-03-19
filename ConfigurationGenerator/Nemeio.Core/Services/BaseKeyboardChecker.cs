using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.Services
{
    abstract class BaseKeyboardChecker : BaseChecker
    {
        private readonly IKeyboardComm _keyboardComm;

        public IKeyboardComm KeyboardComm => _keyboardComm;

        public BaseKeyboardChecker(IKeyboardComm keyboardComm) : base() => _keyboardComm = keyboardComm;
    }
}
