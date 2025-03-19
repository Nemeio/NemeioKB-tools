using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nemeio.Core.Exceptions
{
    public class InvalidUpdateTypeException : Exception
    {
        public InvalidUpdateTypeException() { }

        public InvalidUpdateTypeException(string message) : base(message) { }

        public InvalidUpdateTypeException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidUpdateTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
