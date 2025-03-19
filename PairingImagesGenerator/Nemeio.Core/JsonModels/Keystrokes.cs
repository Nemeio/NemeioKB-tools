using System;
using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.JsonModels
{
    public enum NemeioActionType : sbyte
    {
        Unicode = 0x01,
        Special = 0x02,
        Application = 0x03,
        Url = 0x04,
    }

    public struct NemeioKeyboardPacket
    {
        public UInt16 Header;

        public sbyte Command;

        public short Length;

        public NemeioIndexKeystroke[] Keystrokes;
    }

    public struct NemeioIndexKeystroke
    {
        public int Index;
    }

    public class NemeioKeystroke
    {
        public NemeioIndexKeystroke IndexKeystroke;

        public Key Key;
    }
}
