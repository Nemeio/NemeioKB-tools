using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.DataModels
{
    public class NemeioSerialNumber
    {
        private const int DEFAULT_SERIAL_NUMBER_LENGTH = 12;

        private byte[] _serialNumber;

        public NemeioSerialNumber(byte[] serialNumber)
        {
            if (serialNumber.Length != DEFAULT_SERIAL_NUMBER_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(serialNumber));
            }

            _serialNumber = serialNumber;
        }

        public static bool operator ==(NemeioSerialNumber a, NemeioSerialNumber b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return string.Equals(_serialNumber, ((NemeioSerialNumber)obj)._serialNumber);
        }

        public bool Equals(NemeioSerialNumber x, NemeioSerialNumber y) => x.Equals(y);

        public static bool operator !=(NemeioSerialNumber a, NemeioSerialNumber b) => !(a == b);

        public static implicit operator byte[](NemeioSerialNumber value) => value?._serialNumber;

        public override string ToString() => BitConverter.ToString(_serialNumber);
    }
}
