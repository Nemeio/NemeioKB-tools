using System;
using System.IO;

namespace InstallTool
{
    public static class CommUtils
    {
        public static void writeU32(UInt32 u32, MemoryStream stream)
        {
            byte[] u32Bytes = BitConverter.GetBytes(u32);
            Array.Reverse(u32Bytes);
            stream.Write(u32Bytes, 0, u32Bytes.Length);
        }

        public static UInt32 readU32(BinaryReader reader)
        {
            byte[] u32Bytes = reader.ReadBytes(4);
            Array.Reverse(u32Bytes);
            return BitConverter.ToUInt32(u32Bytes, 0);
        }

        public static void writeU16(UInt16 u16, MemoryStream stream)
        {
            byte[] u16Bytes = BitConverter.GetBytes(u16);
            Array.Reverse(u16Bytes);
            stream.Write(u16Bytes, 0, u16Bytes.Length);
        }

        public static UInt16 readU16(BinaryReader reader)
        {
            byte[] u16Bytes = reader.ReadBytes(2);
            Array.Reverse(u16Bytes);
            return BitConverter.ToUInt16(u16Bytes, 0);
        }

        public static void writeU8(byte u8, MemoryStream stream)
        {
            byte[] u8Byte = new byte[1];
            u8Byte[0] = u8;
            stream.Write(u8Byte, 0, u8Byte.Length);
        }

        public static byte readU8(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static void writeBool(bool b, MemoryStream stream)
        {
            byte byteBool = b ? (byte)0x01 : (byte)0x00;
            writeU8(byteBool, stream);
        }

        public static bool readBool(BinaryReader reader)
        {
            return reader.ReadByte() != 0x00 ? true : false;
        }
    }
}
