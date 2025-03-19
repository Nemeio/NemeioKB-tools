using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.DataModels
{
    [Serializable]
    public class InvalidComPortException : Exception
    {
        public InvalidComPortException()
        {
        }

        public InvalidComPortException(string message) : base(message)
        {
        }

        public InvalidComPortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidComPortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}