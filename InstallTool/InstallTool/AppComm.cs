using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DamienG.Security.Cryptography;
using System.Diagnostics;

namespace InstallTool
{
    class AppComm : IUSBSerialPortListener
    {
        private Crc32 mCRC32;
        private USBSerialPort mSerialPort;
        private List<IAppCommCmdListener> mCmdListeners;
        private List<IAppCommRespListener> mRespListeners;

        private const int AppCommMaxDataSize = InstallToolDefs.ProtocolMaxFrameSize - InstallToolDefs.ProtocolMinFrameSize;

        private struct AppCommFrame
        {
            public byte SOF;
            public byte CmdId;
            public UInt16 Length;
            public byte[] Payload;
            public UInt32 CRC32;
            public byte EOF;
        }

        public AppComm(USBSerialPort serialPort)
        {
            mCRC32 = new Crc32();
            mCmdListeners = new List<IAppCommCmdListener>();
            mRespListeners = new List<IAppCommRespListener>();
            mSerialPort = serialPort;
            mSerialPort.registerListener(this);
        }

        public void registerCmdListener(IAppCommCmdListener listener)
        {
            mCmdListeners.Add(listener);
        }

        public void registerRespListener(IAppCommRespListener listener)
        {
            mRespListeners.Add(listener);
        }

        public bool Send(InstallToolDefs.AppCommCmdID cmdId, byte[] data)
        {
            byte[] frameData;
            bool bRet = GetFrameBytes(cmdId, data, out frameData);
            if (bRet)
            {
                sendData(frameData);
            }

            return bRet;
        }

        public bool GetFrameBytes(InstallToolDefs.AppCommCmdID cmdId, byte[] data, out byte[] frame)
        {
            frame = null;

            if (data.Length > AppCommMaxDataSize)
            {
                return false;
            }

            frame = prepareData(cmdId, data);

            return true;
        }

        public bool SendBytes(byte[] data)
        {
            sendData(data);

            return true;
        }

        private byte[] prepareData(InstallToolDefs.AppCommCmdID cmdId, byte[] payload)
        {
            AppCommFrame frame = new AppCommFrame();
            frame.SOF = InstallToolDefs.ProtocolSOF;
            frame.CmdId = (byte)cmdId;
            frame.Length = (UInt16)payload.Length;
            frame.Payload = payload;
            frame.CRC32 = 0;
            frame.EOF = InstallToolDefs.ProtocolEOF;

            using (var stream = new MemoryStream())
            {
                using (var dataWriter = new BinaryWriter(stream))
                {
                    dataWriter.Write(frame.SOF);

                    long idxCRC32 = stream.Position;

                    dataWriter.Write(frame.CmdId);

                    byte[] lenBytes = BitConverter.GetBytes((UInt16)frame.Length);
                    Array.Reverse(lenBytes);
                    dataWriter.Write(lenBytes);

                    dataWriter.Write(frame.Payload);

                    long crc32DataLen = stream.Position - idxCRC32;
                    var crc32Data = stream.ToArray().Skip((int)idxCRC32).Take((int)crc32DataLen).ToArray();
                    
                    mCRC32.ComputeHash(crc32Data);
                    byte[] crc32Val = mCRC32.Hash;

                    dataWriter.Write(crc32Val);
                    dataWriter.Write(frame.EOF);

                    byte[] frameData = stream.ToArray();

                    return frameData;
                }
            }

            
        }

        private void sendData(byte[] data)
        {
            mSerialPort.Write(data);
        }

        public void DataReceived(byte[] data)
        {
            InstallToolDefs.AppCommCmdID cmdId = InstallToolDefs.AppCommCmdID.SENDDATA;
            bool bResponse = false;
            byte[] payload = null;
            byte[] rxData = data;

            do
            {
                rxData = processRxData(rxData, out cmdId, out bResponse, out payload);
                if (payload != null)
                {
                    if (bResponse)
                    {
                        notifyResponse(cmdId, payload);
                    }
                    else
                    {
                        Debug.Write("Received cmd " + cmdId);
                        notifyCmd(cmdId, payload);
                    }

                }
            } while (rxData != null && rxData.Length > 0);
        }
        
        void notifyResponse(InstallToolDefs.AppCommCmdID cmdId, byte[] payload)
        {
            foreach (var listener in mRespListeners)
            {
                if (listener.CmdId == cmdId)
                {
                    listener.ResponseReceived(payload);
                }
            }
        }

        void notifyCmd(InstallToolDefs.AppCommCmdID cmdId, byte[] payload)
        {
            foreach (var listener in mCmdListeners)
            {
                if (listener.CmdId == cmdId)
                {
                    listener.CmdReceived(payload);
                }
            }
        }


        private byte[] processRxData(byte[] frameData, out InstallToolDefs.AppCommCmdID cmdId, out bool bResponse, out byte[] payload)
        {
            byte[] remainingRxData = null;
            cmdId = InstallToolDefs.AppCommCmdID.SENDDATA;
            bResponse = false;
            payload = null;

            if (frameData.Length < InstallToolDefs.ProtocolMinFrameSize)
            {
                return null;
            }

            AppCommFrame frame = new AppCommFrame();

            using (var stream = new MemoryStream(frameData))
            {
                using (var dataReader = new BinaryReader(stream))
                {
                    frame.SOF = dataReader.ReadByte();

                    frame.CmdId = dataReader.ReadByte();

                    byte[] lengthBytes = dataReader.ReadBytes(2);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBytes);
                    }
                    frame.Length = BitConverter.ToUInt16(lengthBytes, 0);

                    if (frame.Length <= frameData.Length - InstallToolDefs.ProtocolMinFrameSize)
                    {
                        frame.Payload = dataReader.ReadBytes(frame.Length);

                        int crc32Idx = sizeof(byte); // CRC32 starts after SOF
                        int crc32DataLen = sizeof(byte) // CMDID
                                            + sizeof(UInt16) // LEN
                                            + frame.Length; // payload

                        var crc32Data = stream.ToArray().Skip((int)crc32Idx).Take((int)crc32DataLen).ToArray();
                        mCRC32.ComputeHash(crc32Data);

                        byte[] computedCRC32Val = mCRC32.Hash;
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(computedCRC32Val);
                        }
                        UInt32 computedCRC32 = BitConverter.ToUInt32(computedCRC32Val, 0);

                        byte[] crc32Bytes = dataReader.ReadBytes(4);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(crc32Bytes);
                        }
                        frame.CRC32 = BitConverter.ToUInt32(crc32Bytes, 0);

                        frame.EOF = dataReader.ReadByte();

                        if (checkFrame(frame, computedCRC32))
                        {
                            byte cmdIdValue = frame.CmdId;

                            bResponse = (cmdIdValue & InstallToolDefs.ProtocolResponseFlag) != 0 ? true : false;
                            cmdId = (InstallToolDefs.AppCommCmdID)(cmdIdValue & ~InstallToolDefs.ProtocolResponseFlag);
                            payload = frame.Payload;
                            using (var streamRemain = new MemoryStream())
                            {
                                dataReader.BaseStream.CopyTo(streamRemain);
                                remainingRxData = streamRemain.ToArray();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("frame nok");
                        }
                    }

                    return remainingRxData;
                }
            }
        }

        private bool checkFrame(AppCommFrame frame, UInt32 computedCRC32)
        {
            return (frame.SOF == InstallToolDefs.ProtocolSOF
                && frame.EOF == InstallToolDefs.ProtocolEOF
                && frame.CRC32 == computedCRC32);
        }
    }
}
