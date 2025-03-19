using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace InstallTool
{
    class Program
    {
        static void showHelp()
        {
            Console.WriteLine(@"Usage: InstallTool <comnum> <command> [< args >]

Command list:
    DL_FIRMWARE <file>
        Download a new firmware.

    DL_CONFIG <jsonfile> <wallpaperfile>
        Download a user configuration.

    DL_FACTORYCONFIG <jsonfile> <wallpaperfile>
        Download a factory configuration(factory).

    KEEPALIVE <nbreq> <delayreqms>
        Sends keep alive requests.
        Delay is defined in ms.

    APPLYCONFIG <idconfig>
        Applies a configuration.

    CONFIGCHANGED <durationsec>
        Tests the configuration changes for a given duration.
        Duration is defined in seconds.

    KEYPRESSED <durationsec>
        Tests the key presses for a given duration.
        Duration is defined in seconds.

    CFGLIST
        Get the configuration list.

    DELETECONFIG <idconfig>
        Deletes a configuration.

    MULTICMDTEST
        Sends two commands (KeepAlive + ConfigurationList) in the same USB frame.

    FORMATFLASH
        Formats the flash memory.

    BATTERYLVL
        Get the battery level.

    VERSIONS
        Get the firmware versions (CPU + BLE).

    SYSFAILLOG
        Read the System Fail Log.

    SETCOMMMODE
        Tests the SetCommMode command.");
        }
        static void dlFirmware(AppComm appComm, string fileName)
        {
            SendData sendDataCmd = new SendData(appComm);

            sendDataCmd.Send(InstallToolDefs.SendDataID.FIRMWARE, fileName);
        }

        static void sendFirmwarePackage(AppComm appComm, string fileName)
        {
            SendData sendDataCmd = new SendData(appComm);

            sendDataCmd.Send(InstallToolDefs.SendDataID.FIRMWARE_PACKAGE, fileName);
        }

        static void dlInstaller(AppComm appComm, string fileName)
        {
            SendData sendDataCmd = new SendData(appComm);

            sendDataCmd.Send(InstallToolDefs.SendDataID.INSTALLER, fileName);
        }

        static void dlConfiguration(AppComm appComm, string cmd, string cfgJSONFileName, string cfgWallpaperFileName)
        {
            SendData sendCmd = new SendData(appComm);

            InstallToolDefs.SendDataID configDataId;
            InstallToolDefs.SendDataID wallpaperDataId;

            if (cmd.Equals(InstallToolDefs.DLConfig))
            {
                configDataId = InstallToolDefs.SendDataID.CONFIGURATION;
                wallpaperDataId = InstallToolDefs.SendDataID.WALLPAPER;
            }
            else
            {
                configDataId = InstallToolDefs.SendDataID.FACTORYCONFIGURATION;
                wallpaperDataId = InstallToolDefs.SendDataID.FACTORYWALLPAPER;
            }

            if (sendCmd.Send(configDataId, cfgJSONFileName))
            {
                sendCmd.Send(wallpaperDataId, cfgWallpaperFileName);
            }
        }

        static void applyConfiguration(AppComm appComm, string confName)
        {
            ApplyConfiguration applyConfigurationCmd = new ApplyConfiguration(appComm);

            applyConfigurationCmd.apply(confName);
        }

        static void testConfigurationChanged(AppComm appComm, int durationSec)
        {
            ConfigurationChanged confChangedCmd = new ConfigurationChanged(appComm);

            confChangedCmd.test(durationSec);
        }

        static void testKeyPressed(AppComm appComm, int durationSec)
        {
            KeyPressed keyPressedCmd = new KeyPressed(appComm);

            keyPressedCmd.test(durationSec);
        }

        static void keepAlive(AppComm appComm, int nbKeepAlive, int delayMs)
        {
            KeepAlive keepAlive = new KeepAlive(appComm);

            keepAlive.sendKeepAlive(nbKeepAlive, delayMs);
        }

        static void getCfgList(AppComm appComm)
        {
            ConfigurationList configurationListCmd = new ConfigurationList(appComm);

            configurationListCmd.getList();
        }

        static void deleteConfiguration(AppComm appComm, string confName)
        {
            DeleteConfiguration deleteConfigurationCmd = new DeleteConfiguration(appComm);

            deleteConfigurationCmd.delete(confName);
        }

        static void testMultiCmd(AppComm appComm)
        {
            MultiCmdTest multiCmdTest = new MultiCmdTest(appComm);

            multiCmdTest.sendMultiCmd();
        }

        static void testBLEDFU(AppComm appComm)
        {
            BLEDFU bLEDFU = new BLEDFU(appComm);

            bLEDFU.test();
        }

        static void formatFlash(AppComm appComm)
        {
            FormatFlash formatFlashCmd = new FormatFlash(appComm);

            formatFlashCmd.format();
        }

        static void getBatteryLevel(AppComm appComm)
        {
            BatteryStatus batteryLevelCmd = new BatteryStatus(appComm);

            batteryLevelCmd.get();
        }

        static void getVersions(AppComm appComm)
        {
            Versions versionsCmd = new Versions(appComm);

            versionsCmd.get();
        }

        static void getSysFailLog(AppComm appComm)
        {
            SysFailLog sysFailLogCmd = new SysFailLog(appComm);
            sysFailLogCmd.receive();
        }

        static void testSetCommMode(AppComm appComm)
        {
            SetCommMode setCommModeCmd = new SetCommMode(appComm);
            setCommModeCmd.test();
        }

        static void getSerialNumber(AppComm appComm)
        {
            SerialNumber serialNumberCmd = new SerialNumber(appComm);

            serialNumberCmd.get();
        }

        static void setKeyboardParameters(AppComm appComm, KeyboardParameters.KeyboardParametersFromCmd parameters)
        {
            KeyboardParameters keyboardParametersCmd = new KeyboardParameters(appComm);

            keyboardParametersCmd.SetKeyboardParameters(parameters);
        }

        static void getKeyboardParameters(AppComm appComm)
        {
            KeyboardParameters keyboardParametersCmd = new KeyboardParameters(appComm);

            keyboardParametersCmd.GetKeyboardParameters();
        }

        static void factoryReset(AppComm appComm)
        {
            FactoryReset factoryResetCmd = new FactoryReset(appComm);

            factoryResetCmd.reset();
        }

        static void systemReset(AppComm appComm)
        {
            SystemReset systemResetCmd = new SystemReset(appComm);

            systemResetCmd.reset();
        }

        static void testTechnicalError(AppComm appComm, int durationSec)
        {
            TechnicalError technicalErrorCmd = new TechnicalError(appComm);

            technicalErrorCmd.test(durationSec);
        }

        static void displayChipFirmwareUpdate(AppComm appComm, string fileName)
        {
            SendData sendDataCmd = new SendData(appComm);

            sendDataCmd.Send(InstallToolDefs.SendDataID.DISPLAYCHIPFIRMWARE, fileName);
        }

        static string CheckForDevices()
        {
             string VendorId = InstallToolDefs.VendorId.ToString("X4");
             string ProductId = InstallToolDefs.ProductId.ToString("X4");
            string ProductIdNoMSC = InstallToolDefs.ProductIdNoMSC.ToString("X4");
            string ComIdentifier = "";
            var searcher = new ManagementObjectSearcher($"SELECT DeviceID, PNPDeviceID FROM Win32_SerialPort WHERE PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductId}%' OR PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductIdNoMSC}%'");
            foreach (ManagementObject port in searcher.Get())
            {
                ComIdentifier = (string)port["DeviceID"];
            }

            return ComIdentifier;
        }

        static USBSerialPort FindComPort()
        {
            bool bFound = false;
            string ComIdentifier = "";
            while (ComIdentifier.Equals("")) {
                ComIdentifier = CheckForDevices();
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }

            return new USBSerialPort(ComIdentifier);
        }

        static void Main(string[] args)
        {
            bool bArgsOk = true;

            if (args.Length > 0)
            {
                USBSerialPort usbSerialPort = FindComPort();

                Stopwatch stopWatch = new Stopwatch();
          

                AppComm appComm = new AppComm(usbSerialPort);

                if (args.Length == 2 && args[0].Equals(InstallToolDefs.DLFirmware))
                {
                    string fileName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    dlFirmware(appComm, fileName);
                }
                else if (args.Length == 2 && args[0].Equals(InstallToolDefs.FIRMWARE_PACKAGE))
                {
                    string fileName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    sendFirmwarePackage(appComm, fileName);
                }
                else if (args.Length == 3 && (args[0].Equals(InstallToolDefs.DLConfig) || args[0].Equals(InstallToolDefs.DLFactoryConfig)))
                {
                    string cfgJSONFileName = args[1];
                    string cfgWallpaperFileName = args[2];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    dlConfiguration(appComm, args[0], cfgJSONFileName, cfgWallpaperFileName);
                }
                else if (args.Length == 3 && (args[0].Equals(InstallToolDefs.KeepAlive)))
                {
                    int nbKeepAlive = int.Parse(args[1]);
                    int delayMs = int.Parse(args[2]);

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    keepAlive(appComm, nbKeepAlive, delayMs);
                }
                else if (args.Length == 2 && (args[0].Equals(InstallToolDefs.ApplyConfiguration)))
                {
                    string confName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    applyConfiguration(appComm, confName);
                }
                else if (args.Length == 2 && (args[0].Equals(InstallToolDefs.ConfigChanged)))
                {
                    int durationSec = int.Parse(args[1]);

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testConfigurationChanged(appComm, durationSec);
                }
                else if (args.Length == 2 && (args[0].Equals(InstallToolDefs.KeyPressed)))
                {
                    int durationSec = int.Parse(args[1]);

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testKeyPressed(appComm, durationSec);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.ConfigList)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getCfgList(appComm);
                }
                else if (args.Length == 2 && (args[0].Equals(InstallToolDefs.DeleteConfiguration)))
                {
                    string confName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    deleteConfiguration(appComm, confName);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.MultiCmdTest)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testMultiCmd(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.BLEDFUTest)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testBLEDFU(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.FormatFlash)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    formatFlash(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.BatteryLevel)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getBatteryLevel(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.Versions)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getVersions(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.SysFailLog)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getSysFailLog(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.SetCommMode)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testSetCommMode(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.SerialNumber)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getSerialNumber(appComm);
                }
                else if (args.Length == 19 && (args[0].Equals(InstallToolDefs.SetKeyboardParameters)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();

                    KeyboardParameters.KeyboardParametersFromCmd parameters;
                    int i = 1;
                    parameters.inactiveTime = UInt32.Parse(args[i++]);
                    parameters.sleepTime = UInt32.Parse(args[i++]);
                    parameters.inactiveTimeUSBDisconnected = UInt32.Parse(args[i++]);
                    parameters.sleepTimeUSBDisconnected = UInt32.Parse(args[i++]);
                    parameters.powerOffTimeUSBDisconnected = UInt32.Parse(args[i++]);
                    parameters.ledPowerMaxLevel = byte.Parse(args[i++]);
                    parameters.ledPowerInactiveLevel = byte.Parse(args[i++]);
                    parameters.brightnessStep = byte.Parse(args[i++]);
                    parameters.buttonLongPressDelay = UInt16.Parse(args[i++]);
                    parameters.buttonRepeatLongPressDelay = UInt16.Parse(args[i++]);
                    parameters.cleanRefreshPeriod = byte.Parse(args[i++]);
                    parameters.displayLowPowerDelay = byte.Parse(args[i++]);
                    parameters.demoMode = bool.Parse(args[i++]);
                    parameters.lowBatteryBlinkOnDelayMs = UInt16.Parse(args[i++]);
                    parameters.lowBatteryBlinkOffDelayMs = UInt16.Parse(args[i++]);
                    parameters.lowBatteryLevelThresholdPercent = byte.Parse(args[i++]);
                    parameters.bleBlinkOnDelayMs = UInt16.Parse(args[i++]);
                    parameters.bleBlinkOffDelayMs = UInt16.Parse(args[i++]);

                    setKeyboardParameters(appComm, parameters);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.GetKeyboardParameters)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    getKeyboardParameters(appComm);
                }
                else if (args.Length == 1 && (args[0].Equals(InstallToolDefs.FactoryReset)))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    factoryReset(appComm);
                }
                else if (args.Length == 2 && args[0].Equals(InstallToolDefs.DLInstaller))
                {
                    string fileName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    dlInstaller(appComm, fileName);
                }
                else if (args.Length == 1 && args[0].Equals(InstallToolDefs.SystemReset))
                {
                    usbSerialPort.Connect();
                    stopWatch.Start();
                    systemReset(appComm);
                }
                else if (args.Length == 2 && args[0].Equals(InstallToolDefs.TechnicalError))
                {
                    int durationSec = int.Parse(args[1]);

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    testTechnicalError(appComm, durationSec);
                }
                else if (args.Length == 2 && args[0].Equals(InstallToolDefs.DLDisplayChipFirmware))
                {
                    string fileName = args[1];

                    usbSerialPort.Connect();
                    stopWatch.Start();
                    displayChipFirmwareUpdate(appComm, fileName);
                }
                else
                {
                    stopWatch.Start();
                    bArgsOk = false;
                }

                stopWatch.Stop();

                usbSerialPort.Disconnect();

                Console.WriteLine("Time spent: " + stopWatch.Elapsed);
            }
            else
            {
                bArgsOk = false;
            }

            if(!bArgsOk)
            {
                showHelp();
            }
        }
    }
}
