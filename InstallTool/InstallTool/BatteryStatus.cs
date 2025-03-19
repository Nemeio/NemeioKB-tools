using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class BatteryStatus : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID BatteryStatusCmdId = InstallToolDefs.AppCommCmdID.BATTERYSTATUS;
        private const UInt32 INFINITE_TIME_VALUE = UInt32.MaxValue;
        public struct BatteryStatusFromCmd
        {
            public byte statueOfChargePercent;
            public UInt16 remainingCapacityMilliAh;
            public UInt32 timeToFullSec;
            public UInt32 timeToEmptySec;
            public UInt16 cyclesHundredth;
            public byte agePercent;
        };

        private enum ResponseRetCode : byte { SUCCESS = 0 };

        public BatteryStatus(AppComm appComm) : base(appComm, BatteryStatusCmdId)
        {

        }

        public void get()
        {
            byte[] emptyBuff = new byte[0];
            
            mAppComm.Send(BatteryStatusCmdId, emptyBuff);

            getResponse();            
        }

        private string timeToString(UInt32 time)
        {
            return time == INFINITE_TIME_VALUE ? "Infinite" : time.ToString();
        }

        private void displayParameters(BatteryStatusFromCmd status)
        {
            Console.WriteLine(@"Battery status : 
    statueOfChargePercent = {0}
    remainingCapacityMilliAh = {1}
    timeToFullSec = {2}
    timeToEmptySec = {3}
    cyclesHundredth = {4}
    agePercent = {5}", status.statueOfChargePercent,
                        status.remainingCapacityMilliAh,
                        timeToString(status.timeToFullSec),
                        timeToString(status.timeToEmptySec),
                        status.cyclesHundredth,
                        status.agePercent);
        }

        private bool getResponse()
        {
            bool bRet = false;
            byte[] response = waitResponse();

            if (response != null && response.Length >= 1)
            {
                bRet = true;
                ResponseRetCode respRetCode = (ResponseRetCode)response[0];
                if (respRetCode == ResponseRetCode.SUCCESS && response.Length >= 15)
                {
                    using (var responseStream = new MemoryStream(response.Skip(1).ToArray()))
                    {
                        using (var responseBinReader = new BinaryReader(responseStream))
                        {
                            BatteryStatusFromCmd status;
                            status.statueOfChargePercent = CommUtils.readU8(responseBinReader);
                            status.remainingCapacityMilliAh = CommUtils.readU16(responseBinReader);
                            status.timeToFullSec = CommUtils.readU32(responseBinReader);
                            status.timeToEmptySec = CommUtils.readU32(responseBinReader);
                            status.cyclesHundredth = CommUtils.readU16(responseBinReader);
                            status.agePercent = CommUtils.readU8(responseBinReader);

                            displayParameters(status);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Received error " + respRetCode);
                }
            }
            else
            {
                Console.WriteLine("Error receiving response");
            }

            return bRet;
        }

    }
}
