using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nemeio.Core.Exceptions
{
    public class TooLargeItemException : Exception
    {
        public TooLargeItemException() { }

        public TooLargeItemException(string message) : base(message) { }

        public TooLargeItemException(string message, Exception innerException) : base(message, innerException) { }

        protected TooLargeItemException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
