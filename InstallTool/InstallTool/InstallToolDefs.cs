using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class InstallToolDefs
    {
        public const string DLFirmware = "DL_FIRMWARE";
        public const string DLConfig = "DL_CONFIG";
        public const string DLFactoryConfig = "DL_FACTORYCONFIG";
        public const string KeepAlive = "KEEPALIVE";
        public const string ApplyConfiguration = "APPLYCONFIG";
        public const string ConfigChanged = "CONFIGCHANGED";
        public const string KeyPressed = "KEYPRESSED";
        public const string ConfigList = "CFGLIST";
        public const string DeleteConfiguration = "DELETECONFIG";
        public const string MultiCmdTest = "MULTICMDTEST";
        public const string BLEDFUTest = "BLEDFUTEST";
        public const string FormatFlash = "FORMATFLASH";
        public const string BatteryLevel = "BATTERYSTATUS";
        public const string Versions = "VERSIONS";
        public const string SysFailLog = "SYSFAILLOG";
        public const string SetCommMode = "SETCOMMMODE";
        public const string SerialNumber = "SERIALNUMBER";
        public const string GetKeyboardParameters = "GETKEYBOARDPARAMETERS";
        public const string SetKeyboardParameters = "SETKEYBOARDPARAMETERS";
        public const string FactoryReset = "FACTORYRESET";
        public const string DLInstaller = "DL_INSTALLER";
        public const string SystemReset = "SYSTEMRESET";
        public const string TechnicalError = "TECHNICALERROR";
        public const string DLDisplayChipFirmware = "DL_DISPLAYCHIPFIRMWARE";
        public const string FIRMWARE_PACKAGE = "PACKAGE";
        public static readonly long VendorId = 0x0483;
        public static readonly long ProductId = 0x1234;
        public static readonly long ProductIdNoMSC = 0x1235;
        public enum AppCommCmdID : byte { SENDDATA = 0,
            KEEPALIVE = 1,
            APPLYCONFIG = 2,
            CONFIGCHANGED = 3,
            KEYPRESSED = 4,
            CFGLIST = 5,
            DELETECONFIG = 6,
            BLEDFU = 7,
            FORMATFLASH = 8,
            BATTERYSTATUS = 9,
            VERSIONS = 10,
            RECEIVEDATA = 11,
            SETCOMMMODE = 12,
            SERIALNUMBER = 13,
            KEYBOARDPARAMETERS = 14,
            FACTORYRESET = 15,
            SYSTEMRESET = 16,
            TECHNICALERROR = 17,
        };

        public enum SendDataID : byte 
        { 
            FIRMWARE = 1,
            FACTORYCONFIGURATION = 2,
            CONFIGURATION = 3,
            FACTORYWALLPAPER = 4,
            WALLPAPER = 5,
            INSTALLER = 6,
            DISPLAYCHIPFIRMWARE = 7,
            FIRMWARE_PACKAGE = 8
        };

        public enum ReceiveDataID : byte
        {
            SYSFAILLOG = 1,
        };

        public const byte ProtocolSOF = 0x01;
        public const byte ProtocolEOF = 0x04;
        public const int ProtocolMinFrameSize = sizeof(byte) // SOF
                                                    + sizeof(byte) // CMDID
                                                    + sizeof(UInt16) // LEN
                                                    + sizeof(UInt32) // CRC32
                                                    + sizeof(byte); // EOF
        public const int ProtocolMaxFrameSize = 1024;
        public const byte ProtocolResponseFlag = 0x80;

        public const byte UsbPacketSize = 0x40;

        public const int ConfigNameSize = 36;

    }
}
