using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class Versions : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID VersionsCmdId = InstallToolDefs.AppCommCmdID.VERSIONS;
        private const int VersionMaxLength = 16;
        private enum ResponseRetCode : sbyte { SUCCESS = 0 };


        public Versions(AppComm appComm) : base(appComm, VersionsCmdId)
        {

        }

        public void get()
        {
            byte[] emptyBuff = new byte[0];

            mAppComm.Send(VersionsCmdId, emptyBuff);

            getResponse();
        }

        private string parseString(byte[] buffer)
        {
            string retStr = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            var nullIdx = retStr.IndexOf('\0');
            if(nullIdx >= 0)
            {
                retStr = retStr.Substring(0, retStr.IndexOf('\0'));
            }
            return retStr;
        }

        private bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();

            if (response != null && response.Length >= VersionMaxLength * 2 + sizeof(ResponseRetCode))
            {
                using (var versionResponse = new MemoryStream(response))
                {
                    using (var binVersionResponse = new BinaryReader(versionResponse))
                    {
                        bRet = true;

                        ResponseRetCode respRetCode = (ResponseRetCode)binVersionResponse.ReadByte();
                        if(respRetCode != ResponseRetCode.SUCCESS)
                        {
                            bRet = false;
                            Console.WriteLine("\r\nReceived error CPU " + respRetCode);
                        }
                        string cpuVersion = parseString(binVersionResponse.ReadBytes(VersionMaxLength));

                        respRetCode = (ResponseRetCode)binVersionResponse.ReadByte();
                        if (respRetCode != ResponseRetCode.SUCCESS)
                        {
                            bRet = false;
                            Console.WriteLine("\r\nReceived error BLE " + respRetCode);
                        }
                        string bleVersion = parseString(binVersionResponse.ReadBytes(VersionMaxLength));

                        respRetCode = (ResponseRetCode)binVersionResponse.ReadByte();
                        if (respRetCode != ResponseRetCode.SUCCESS)
                        {
                            bRet = false;
                            Console.WriteLine("\r\nReceived error Display " + respRetCode);
                        }
                        string displayChipFirmwareVersion = parseString(binVersionResponse.ReadBytes(VersionMaxLength));
                        string displayChipLUTVersion = parseString(binVersionResponse.ReadBytes(VersionMaxLength));

                        Console.WriteLine("CPU Version = {0}", cpuVersion);
                        Console.WriteLine("BLE Version = {0}", bleVersion);
                        Console.WriteLine("Display Chip Firmware Version = {0}", displayChipFirmwareVersion);
                        Console.WriteLine("Display Chip LUT Version (Waveforms) = {0}", displayChipLUTVersion);
                    }
                }


                //    string cpuVersion = parseString(response.Take(VersionMaxLength).ToArray());
                //string bleVersion = parseString(response.Skip(VersionMaxLength).Take(VersionMaxLength).ToArray());
                //Console.WriteLine("CPU Version = {0}", cpuVersion);
                //Console.WriteLine("BLE Version = {0}", bleVersion);
            }
            else
            {
                Console.WriteLine("\r\nError receiving response");
            }

            return bRet;
        }
    }
}
