using Nemeio.Core.Extensions;
using System;
using System.IO;

namespace Nemeio.Core
{
    public static class NemeioConstants
    {
        public static readonly string KeyboardIp                    = "http://10.0.0.1/"; //Keyboard
        public static readonly string ServerUrl                     = "http://colomba.moessner.fr";
        public static readonly string FirmwareUrl                   = ServerUrl + "/versions.php";

        public static readonly long VendorId                        = 0x0483;
        public static readonly long ProductId                       = 0x1234;

        public static readonly int NemeioKeepAliveTimeout           = 1000;
        public static readonly int NemeioUpdateVersionTimeout       = 86400000; // 24h
        public static readonly int NemeioBatteryTimeout             = 60000;

        public static readonly int NemeioDefaultDelay               = 500;
        public static readonly int NemeioDefaultSpeed               = 40;

        public static readonly int NemeioMinimumBatteryLevel        = 10;
        public static readonly ushort NemeioBatteryNotPlugged       = 0;

        public static readonly int NotificationTimeout              = 5000;

        public static readonly string AppName                       = "Nemeio";
        public static readonly string LogFolderName                 = "logs";
        public static readonly string LogFileName                   = "nemeio";
        public static readonly string KeyboardCrashFileName         = "kbcrash";
        public static readonly string LogExtension                  = ".log";

        public static readonly string TemporaryDirectoryName        = "tmp";

        public static readonly int DefaultCategoryId                = 123;

        private static readonly string MacLibraryName               = "Library";
        private static readonly string MacApplicationSupportName    = "Application Support";

        public static string LogPath
        {
            get
            {
                if (ObjectExtensions.IsOSXPlatform())
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        MacLibraryName,
                        MacApplicationSupportName,
                        AppName,
                        LogFolderName
                    );
                }
                else
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        AppName,
                        LogFolderName
                    );
                }
            }
        }
    }
}
