using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class ReceiveData : AppCommCmd
    {
        private enum ReceiveDataCmdID : byte { START = 0, RECEIVE = 1, STOP = 2 };

        private const InstallToolDefs.AppCommCmdID ReceiveDataCmdId = InstallToolDefs.AppCommCmdID.RECEIVEDATA;
        private const int ReceiveDataMaxLen = 128;
        
        private enum ResponseRetCode : sbyte { SUCCESS = 0 };

        private struct ReceiveDataFrame
        {
            public byte DataId;
            public byte CmdId;
            public byte[] Payload;
        }

        public ReceiveData(AppComm appComm) : base(appComm, ReceiveDataCmdId)
        {
        }

        public bool Receive(InstallToolDefs.ReceiveDataID dataId, out byte[] receiveData)
        {
            receiveData = new byte[0];

            bool bRet = true;

            int dataLength;
            bRet = start(dataId, out dataLength);

            int remaininingDataLength = dataLength;
            int idxData = 0;
            showProgress(idxData, dataLength);
            while (bRet && remaininingDataLength > 0)
            {
                int frameMaxDataSize = Math.Min(ReceiveDataMaxLen, remaininingDataLength);

                byte[] dataChunk = new byte[0];
                bRet = receive(dataId, idxData, frameMaxDataSize, out dataChunk);
                receiveData = receiveData.Concat(dataChunk).ToArray();

                idxData += dataChunk.Length;
                remaininingDataLength -= dataChunk.Length;
                showProgress(idxData, dataLength);
            }

            if (bRet)
            {
                bRet = stop(dataId);
            }

            Console.WriteLine();

            return bRet;
        }

        private void showProgress(int idxData, int length)
        {
            if (length == 0)
            {
                return;
            }

            int perProgress = (idxData * 100) / length;
            if (perProgress % 10 == 0)
            {
                Console.Write("\r{0}%", perProgress);
            }
        }

        private bool start(InstallToolDefs.ReceiveDataID dataId, out int dataLength)
        {
            byte[] emptyData = new byte[0];
            dataLength = 0;
            bool bRet = transmitData(dataId, ReceiveDataCmdID.START, emptyData);
            if(bRet)
            {
                bRet = getStartResponse(out dataLength);
            }

            return bRet;
        }

        private bool receive(InstallToolDefs.ReceiveDataID dataId, int offset, int receiveMaxSize, out byte[] dataChunk)
        {
            dataChunk = new byte[0];

            using (var stream = new MemoryStream())
            {
                UInt32 offsetU32 = (UInt32)offset;
                byte[] offsetBytes = BitConverter.GetBytes(offsetU32);
                Array.Reverse(offsetBytes);
                stream.Write(offsetBytes, 0, offsetBytes.Length);

                UInt32 maxReceiveSizeU32 = (UInt32) receiveMaxSize;
                byte[] maxReceiveSizeBytes = BitConverter.GetBytes(maxReceiveSizeU32);
                Array.Reverse(maxReceiveSizeBytes);
                stream.Write(maxReceiveSizeBytes, 0, maxReceiveSizeBytes.Length);

                byte[] sendBytes = stream.ToArray();

                bool bRet = transmitData(dataId, ReceiveDataCmdID.RECEIVE, sendBytes);
                if(bRet)
                {
                    bRet = getReceiveResponse(out dataChunk);
                }
                return bRet;
            }
        }

        private bool stop(InstallToolDefs.ReceiveDataID dataId)
        {
            byte[] emptyData = new byte[0];
            bool bRet = transmitData(dataId, ReceiveDataCmdID.STOP, emptyData);
            if (bRet)
            {
                bRet = getStopResponse();
            }

            return bRet;
        }

        private bool transmitData(InstallToolDefs.ReceiveDataID dataId, ReceiveDataCmdID cmdId, byte[] data)
        {
            ReceiveDataFrame rcvDataFrame = new ReceiveDataFrame();
            rcvDataFrame.DataId = (byte)dataId;
            rcvDataFrame.CmdId = (byte)cmdId;
            rcvDataFrame.Payload = data;

            using (var stream = new MemoryStream())
            {
                using (var dataWriter = new BinaryWriter(stream))
                {
                    dataWriter.Write(rcvDataFrame.DataId);
                    dataWriter.Write(rcvDataFrame.CmdId);
                    dataWriter.Write(rcvDataFrame.Payload);

                    byte[] frameBytes = stream.ToArray();

                    mAppComm.Send(ReceiveDataCmdId, frameBytes);

                    return true;
                }
            }
        }
        
        bool getStartResponse(out int receiveDataLength) {
            bool bRet = false;
            receiveDataLength = 0;
            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(byte))
            {
                using (var startResponse = new MemoryStream(response))
                {
                    using (var binStartResponse = new BinaryReader(startResponse))
                    {
                        ResponseRetCode respRetCode = (ResponseRetCode)binStartResponse.ReadByte();
                        if (respRetCode == ResponseRetCode.SUCCESS)
                        {
                            bRet = true;
                            byte[] sizeBytes = binStartResponse.ReadBytes(4);
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(sizeBytes);
                            receiveDataLength = BitConverter.ToInt32(sizeBytes, 0);
                        }
                        else
                        {
                            Console.WriteLine("\r\nReceived error " + respRetCode);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\r\nError receiving response");
            }

            return bRet;
        }

        bool getReceiveResponse(out byte[] receiveData)
        {
            bool bRet = false;
            receiveData = new byte[0];

            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(byte))
            {
                using (var startResponse = new MemoryStream(response))
                {
                    using (var binStartResponse = new BinaryReader(startResponse))
                    {
                        ResponseRetCode respRetCode = (ResponseRetCode)binStartResponse.ReadByte();
                        if (respRetCode == ResponseRetCode.SUCCESS)
                        {
                            bRet = true;
                            int receiveLength = response.Length - sizeof(byte);
                            receiveData = binStartResponse.ReadBytes(receiveLength);
                        }
                        else
                        {
                            Console.WriteLine("\r\nReceived error " + respRetCode);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\r\nError receiving response");
            }

            return bRet;
        }

        private bool getStopResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(byte))
            {
                ResponseRetCode respRetCode = (ResponseRetCode)response[0];
                if (respRetCode == ResponseRetCode.SUCCESS)
                {
                    bRet = true;
                }
                else
                {
                    Console.WriteLine("\r\nReceived error " + respRetCode);
                }
            }
            else
            {
                Console.WriteLine("\r\nError receiving response");
            }

            return bRet;
        }
    }
}
