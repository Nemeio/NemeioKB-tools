using System;
using System.IO;
using System.Linq;

namespace InstallTool
{
    class KeyboardParameters : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID KeyboardParametersCmdId = InstallToolDefs.AppCommCmdID.KEYBOARDPARAMETERS;

        private enum ResponseRetCode : byte { SUCCESS = 0 };
        private enum SubCmdId : byte { UNEDEFINED = 0, GET, SET };

        public struct KeyboardParametersFromCmd
        {
            public UInt32 inactiveTime;
            public UInt32 sleepTime;
            public UInt32 inactiveTimeUSBDisconnected;
            public UInt32 sleepTimeUSBDisconnected;
            public UInt32 powerOffTimeUSBDisconnected;
            public byte ledPowerMaxLevel;
            public byte ledPowerInactiveLevel;
            public byte brightnessStep;
            public UInt16 buttonLongPressDelay;
            public UInt16 buttonRepeatLongPressDelay;
            public byte cleanRefreshPeriod;
            public UInt16 displayLowPowerDelay;
            public bool demoMode;
            public UInt16 lowBatteryBlinkOnDelayMs;
            public UInt16 lowBatteryBlinkOffDelayMs;
            public byte lowBatteryLevelThresholdPercent;
            public UInt16 bleBlinkOnDelayMs;
            public UInt16 bleBlinkOffDelayMs;
        };

        public KeyboardParameters(AppComm appComm) : base(appComm, KeyboardParametersCmdId)
        {
        }

        private void displayParameters(KeyboardParametersFromCmd parameters)
        {
            Console.WriteLine(@"Keyboard parameters : 
    inactiveTime = {0}
    sleepTime = {1}
    inactiveTimeUSBDisconnected = {2}
    sleepTimeUSBDisconnected = {3}
    powerOffTimeUSBDisconnected = {4}
    ledPowerMaxLevel = {5}
    ledPowerInactiveLevel = {6}
    brightnessStep = {7}
    buttonLongPressDelay = {8}
    buttonRepeatLongPressDelay = {9}
    cleanRefreshPeriod = {10}
    displayLowPowerDelay = {11}
    demoMode = {12}
    lowBatteryBlinkOnDelayMs = {13}
    lowBatteryBlinkOffDelayMs = {14}
    lowBatteryLevelThresholdPercent = {15}
    bleBlinkOnDelayMs = {16}
    bleBlinkOffDelayMs = {17}", parameters.inactiveTime,
                        parameters.sleepTime,
                        parameters.inactiveTimeUSBDisconnected,
                        parameters.sleepTimeUSBDisconnected,
                        parameters.powerOffTimeUSBDisconnected,
                        parameters.ledPowerMaxLevel,
                        parameters.ledPowerInactiveLevel,
                        parameters.brightnessStep,
                        parameters.buttonLongPressDelay,
                        parameters.buttonRepeatLongPressDelay,
                        parameters.cleanRefreshPeriod,
                        parameters.displayLowPowerDelay,
                        parameters.demoMode,
                        parameters.lowBatteryBlinkOnDelayMs,
                        parameters.lowBatteryBlinkOffDelayMs,
                        parameters.lowBatteryLevelThresholdPercent,
                        parameters.bleBlinkOnDelayMs,
                        parameters.bleBlinkOffDelayMs);
        }

        public bool SetKeyboardParameters(KeyboardParametersFromCmd parameters)
        {
            displayParameters(parameters);

            using (var stream = new MemoryStream())
            {
                CommUtils.writeU8((byte)SubCmdId.SET, stream);
                CommUtils.writeU32(parameters.inactiveTime, stream);
                CommUtils.writeU32(parameters.sleepTime, stream);
                CommUtils.writeU32(parameters.inactiveTimeUSBDisconnected, stream);
                CommUtils.writeU32(parameters.sleepTimeUSBDisconnected, stream);
                CommUtils.writeU32(parameters.powerOffTimeUSBDisconnected, stream);
                CommUtils.writeU8(parameters.ledPowerMaxLevel, stream);
                CommUtils.writeU8(parameters.ledPowerInactiveLevel, stream);
                CommUtils.writeU8(parameters.brightnessStep, stream);
                CommUtils.writeU16(parameters.buttonLongPressDelay, stream);
                CommUtils.writeU16(parameters.buttonRepeatLongPressDelay, stream);
                CommUtils.writeU8(parameters.cleanRefreshPeriod, stream);
                CommUtils.writeU16(parameters.displayLowPowerDelay, stream);
                CommUtils.writeBool(parameters.demoMode, stream);
                CommUtils.writeU16(parameters.lowBatteryBlinkOnDelayMs, stream);
                CommUtils.writeU16(parameters.lowBatteryBlinkOffDelayMs, stream);
                CommUtils.writeU8(parameters.lowBatteryLevelThresholdPercent, stream);
                CommUtils.writeU16(parameters.bleBlinkOnDelayMs, stream);
                CommUtils.writeU16(parameters.bleBlinkOffDelayMs, stream);

                byte[] frameBytes = stream.ToArray();

                bool bRet = mAppComm.Send(KeyboardParametersCmdId, frameBytes);
                
                if (bRet)
                {
                    bRet = getResponse();
                }
                return bRet;
            }
        }

        public bool GetKeyboardParameters()
        {
            using (var stream = new MemoryStream())
            {
                CommUtils.writeU8((byte)SubCmdId.GET, stream);

                byte[] frameBytes = stream.ToArray();

                bool bRet = mAppComm.Send(KeyboardParametersCmdId, frameBytes);

                if (bRet)
                {
                    bRet = getResponse();
                }
                return bRet;
            }
        }

        bool parseGetParametersResponse(byte[] response)
        {
            bool bRet = false;
            if (response != null && response.Length >= 31)
            {
                bRet = true;
                using (var responseStream = new MemoryStream(response))
                {
                    using (var responseBinReader = new BinaryReader(responseStream))
                    {
                        KeyboardParametersFromCmd parameters;
                        parameters.inactiveTime = CommUtils.readU32(responseBinReader);
                        parameters.sleepTime = CommUtils.readU32(responseBinReader);
                        parameters.inactiveTimeUSBDisconnected = CommUtils.readU32(responseBinReader);
                        parameters.sleepTimeUSBDisconnected = CommUtils.readU32(responseBinReader);
                        parameters.powerOffTimeUSBDisconnected = CommUtils.readU32(responseBinReader);
                        parameters.ledPowerMaxLevel = CommUtils.readU8(responseBinReader);
                        parameters.ledPowerInactiveLevel = CommUtils.readU8(responseBinReader);
                        parameters.brightnessStep = CommUtils.readU8(responseBinReader);
                        parameters.buttonLongPressDelay = CommUtils.readU16(responseBinReader);
                        parameters.buttonRepeatLongPressDelay = CommUtils.readU16(responseBinReader);
                        parameters.cleanRefreshPeriod = CommUtils.readU8(responseBinReader);
                        parameters.displayLowPowerDelay = CommUtils.readU16(responseBinReader);
                        parameters.demoMode = CommUtils.readBool(responseBinReader);
                        parameters.lowBatteryBlinkOnDelayMs = CommUtils.readU16(responseBinReader);
                        parameters.lowBatteryBlinkOffDelayMs = CommUtils.readU16(responseBinReader);
                        parameters.lowBatteryLevelThresholdPercent = CommUtils.readU8(responseBinReader);
                        parameters.bleBlinkOnDelayMs = CommUtils.readU16(responseBinReader);
                        parameters.bleBlinkOffDelayMs = CommUtils.readU16(responseBinReader);

                        displayParameters(parameters);
                    }
                }
            }
            else
            {
                Console.WriteLine("Not enough data received");
            }

            return bRet;
        }

        private bool getResponse()
        {
            bool bRet = false;

            byte[] response = waitResponse();
            if (response != null && response.Length >= sizeof(ResponseRetCode) + sizeof(SubCmdId))
            {
                SubCmdId cmdId = (SubCmdId)response[0];
                ResponseRetCode respRetCode = (ResponseRetCode)response[1];
                if (respRetCode == ResponseRetCode.SUCCESS)
                {
                    bRet = true;

                    switch(cmdId)
                    {
                        case SubCmdId.GET:
                            if (parseGetParametersResponse(response.Skip(2).ToArray())) {
                                Console.WriteLine("Parameters successfully gotten");
                            }
                            break;
                        case SubCmdId.SET:
                            Console.WriteLine("Parameters successfully set");
                            break;
                        default:
                            break;
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
