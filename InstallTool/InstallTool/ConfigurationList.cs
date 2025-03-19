using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class ConfigurationList : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID ConfigurationListCmdId = InstallToolDefs.AppCommCmdID.CFGLIST;
        private enum ConfigurationListCmdID : byte { COUNT = 0, NAME = 1};

        public ConfigurationList(AppComm appComm) : base(appComm, ConfigurationListCmdId)
        {

        }

        public void getList()
        {
            byte[] emptyBuff = new byte[0];

            mAppComm.Send(ConfigurationListCmdId, emptyBuff);
            getResponse();
        }

        public byte[] getFrame()
        {
            byte[] emptyBuff = new byte[0];
            byte[] frameBuff;
            mAppComm.GetFrameBytes(ConfigurationListCmdId, emptyBuff, out frameBuff);

            return frameBuff;
        }

        public bool getResponse()
        {
            bool bRet = true;
            uint cfgCount;
            if (getCfgCount(out cfgCount))
            {
                Console.WriteLine("Number of configurations: {0}", cfgCount);
                for (int i = 0; i < cfgCount; i++)
                {
                    string cfgName;
                    if (getCfgName(out cfgName))
                    {
                        Console.WriteLine("{0}: {1}", i, cfgName);
                    }
                    else
                    {
                        bRet = false;
                        break;
                    }
                }
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        private bool getCfgCount(out uint count)
        {
            bool bRet = false;
            count = 0;

            byte[] response = waitResponse();
            if (response != null && response.Length == sizeof(ConfigurationListCmdID) + sizeof(UInt32))
            {
                bRet = true;
            }

            if (bRet && (byte)ConfigurationListCmdID.COUNT == response[0])
            {
                var countArray = response.Skip(sizeof(ConfigurationListCmdID)).ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(countArray);
                }
                count = BitConverter.ToUInt32(countArray, 0);
            }
            else
            {
                Console.WriteLine("Error receiving response");
            }

            return bRet;
        }

        private bool getCfgName(out string name)
        {
            bool bRet = false;
            name = "";

            byte[] response = waitResponse();
            if (response != null && response.Length == sizeof(ConfigurationListCmdID) + InstallToolDefs.ConfigNameSize)
            {
                bRet = true;
            }

            if (bRet && (byte)ConfigurationListCmdID.NAME == response[0])
            {
                var nameArray = response.Skip(sizeof(ConfigurationListCmdID)).ToArray();
                name = System.Text.Encoding.Default.GetString(nameArray);
            }
            else
            {
                Console.WriteLine("Error receiving response");
            }

            return bRet;
        }
    }
}
