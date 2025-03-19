using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace InstallTool
{
    class SendData : AppCommCmd
    {
        private enum SendDataCmdID : byte { START = 0, SEND = 1, STOP = 2 };

        private const InstallToolDefs.AppCommCmdID SendDataCmdId = InstallToolDefs.AppCommCmdID.SENDDATA;

        private const int SendDataSendMinFrameSize = InstallToolDefs.ProtocolMinFrameSize
                                                + sizeof(byte) // DATAID
                                                + sizeof(byte); // CMDID

        private const int SendDataSendMaxDataSize = InstallToolDefs.ProtocolMaxFrameSize - SendDataSendMinFrameSize
                                                - sizeof(UInt32); // OFFSET

        private enum ResponseRetCode : sbyte { SUCCESS = 0 };

        private const int SendDataAckSize = SendDataSendMinFrameSize + sizeof(ResponseRetCode);

        private struct SendDataFrame
        {
            public byte DataId;
            public byte CmdId;
            public byte[] Payload;
        }

        public SendData(AppComm appComm) : base(appComm, SendDataCmdId)
        {
        }

        public bool Send(InstallToolDefs.SendDataID dataId, string fileName)
        {
            bool bRet = false;
            FileInfo fi = new FileInfo(fileName);
            UInt32 size = (UInt32)fi.Length;

            FileStream fsInput = new FileStream(fileName, FileMode.Open);
            byte[] fileBytes = new byte[size];

            fsInput.Read(fileBytes, 0, fileBytes.Length);

            bRet = doSendData(dataId, fileBytes);
            fsInput.Close();
            return bRet;
        }

        private bool doSendData(InstallToolDefs.SendDataID dataId, byte[] data)
        {
            bool bRet = true;
            
            bRet = start(dataId, data);

            int remaininingDataLength = data.Length;
            int idxData = 0;

            showProgress(idxData, data.Length);
            while (bRet && remaininingDataLength > 0)
            {
                int frameDataSize = Math.Min(SendDataSendMaxDataSize, remaininingDataLength);
                var dataChunk = data.Skip(idxData).Take(frameDataSize).ToArray();

                bRet = send(dataId, idxData, dataChunk);
                idxData += frameDataSize;
                remaininingDataLength -= frameDataSize;
                showProgress(idxData,data.Length);
            }

            if(bRet)
            {
                bRet = stop(dataId);
            }
            
            Console.WriteLine();

            return bRet;
        }

        private void showProgress(int idxData, int length)
        {
            int perProgress = (idxData * 100) / length;
            if(perProgress % 10 == 0)
            {
                Console.Write("\r{0}%", perProgress);
            }
        }

        private bool start(InstallToolDefs.SendDataID dataId, byte[] data)
        {
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            Array.Reverse(lengthBytes);
            return transmitData(dataId, SendDataCmdID.START, lengthBytes);
        }

        private bool send(InstallToolDefs.SendDataID dataId, int offset, byte[] dataChunk)
        {
            using (var stream = new MemoryStream())
            {
                UInt32 offsetU32 = (UInt32)offset;
                byte[] offsetBytes = BitConverter.GetBytes(offsetU32);
                Array.Reverse(offsetBytes);
                stream.Write(offsetBytes, 0, offsetBytes.Length);
                stream.Write(dataChunk, 0, dataChunk.Length);

                byte[] sendBytes = stream.ToArray();

                return transmitData(dataId, SendDataCmdID.SEND, sendBytes);
            }
        }

        private bool stop(InstallToolDefs.SendDataID dataId)
        {
            byte[] emptyData = new byte[0];
            return transmitData(dataId, SendDataCmdID.STOP, emptyData);
        }

        private bool transmitData(InstallToolDefs.SendDataID dataId, SendDataCmdID cmdId, byte[] data)
        {
            SendDataFrame sendDataFrame = new SendDataFrame();
            sendDataFrame.DataId = (byte)dataId;
            sendDataFrame.CmdId = (byte)cmdId;
            sendDataFrame.Payload = data;

            using (var stream = new MemoryStream())
            {
                using (var dataWriter = new BinaryWriter(stream))
                {
                    dataWriter.Write(sendDataFrame.DataId);
                    dataWriter.Write(sendDataFrame.CmdId);
                    dataWriter.Write(sendDataFrame.Payload);

                    byte[] frameBytes = stream.ToArray();

                    mAppComm.Send(SendDataCmdId, frameBytes);
                    
                    return getResponse();
                }
            }
        }

        private bool getResponse()
        {
            bool bRet = false;
            
            byte[] response = waitResponse();
            if(response != null && response.Length >= sizeof(byte))
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
