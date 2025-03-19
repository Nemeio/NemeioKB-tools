using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nemeio.Core.DataModels
{
    public class BatteryLevel : UShortType<BatteryLevel>
    {
        public BatteryLevel(ushort value) : base(value) { }

        public BatteryLevel(byte[] value) : base(GetValueFromBytes(value)) { }

        private static ushort GetValueFromBytes(byte[] payload)
        {
            if (payload.Length > 0)
            {
                var val = payload[0];
                if (val >= 0 && val <= 100)
                {
                    return val;
                }
                else
                {
                    return NemeioConstants.NemeioBatteryNotPlugged;
                }
            }
            else
            {
                throw new InvalidDataException(nameof(payload));
            }
        }

        public static BatteryLevel NotPlugged => new BatteryLevel(NemeioConstants.NemeioBatteryNotPlugged);
    }
}
