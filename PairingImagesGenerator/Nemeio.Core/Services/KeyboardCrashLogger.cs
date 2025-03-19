using Nemeio.Core.DataModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nemeio.Core.Services
{
    internal class KeyboardCrashLogger
    {
        private static int _assertFailLabelSizeAlignment = 10;
        private static int _faultExceptionLabelSizeAlignment = 13;

        // due to start up mecanisms, need to build path here like in IDocument without using IDocument itself
        private static string _logFolderPath = NemeioConstants.LogPath;

        public KeyboardCrashLogger()
        {
            // sanity
            if (!Directory.Exists(_logFolderPath))
            {
                Directory.CreateDirectory(_logFolderPath);
            }
        }

        public void WriteKeyboardCrashLog(IList<KeyboardFailure> keyboardFailures)
        {
            // sanity: prevent empty log creation
            if (keyboardFailures.Count==0)
            {
                return;
            }

            // formatting string to better align values in log file
            string fileName = $"{NemeioConstants.KeyboardCrashFileName}{DateTime.Now.ToString("yyyyMMdd-hhmmss")}{NemeioConstants.LogExtension}";
            using (StreamWriter writer = new StreamWriter(Path.Combine(_logFolderPath, fileName)))
            {
                foreach(KeyboardFailure keyboardFailure in keyboardFailures)
                {
                    int labelSize = _assertFailLabelSizeAlignment;
                    if (keyboardFailure.Id == KeyboardEventId.FaultExceptionEvent)
                    {
                        labelSize = _faultExceptionLabelSizeAlignment;
                    }
                    writer.WriteLine(FormatLabel("EventId", labelSize) + keyboardFailure.Id.ToString());
                    for (int i = 0; i < KeyboardFailure.NumberOfRegistries; i++)
                    {
                        writer.WriteLine(FormatLabel(string.Format("R{0}", i), labelSize) + FormatUInt32(keyboardFailure.Registries[i]));
                    }
                    writer.WriteLine(FormatLabel("SP", labelSize) + FormatUInt32(keyboardFailure.SP));
                    writer.WriteLine(FormatLabel("LR", labelSize) + FormatUInt32(keyboardFailure.LR));
                    writer.WriteLine(FormatLabel("PC", labelSize) + FormatUInt32(keyboardFailure.PC));
                    writer.WriteLine(FormatLabel("xPSR", labelSize) + FormatUInt32(keyboardFailure.xPSR));
                    if (keyboardFailure.Id == KeyboardEventId.FaultExceptionEvent)
                    {
                        writer.WriteLine(FormatLabel("exceptType", labelSize) + keyboardFailure.ExceptionType.ToString());
                    }
                    writer.WriteLine("===");
                }
            }
        }

        private string FormatLabel(string label, int size)
        {
            return string.Format(@"{0} = ", label).PadLeft(size);
        }

        private string FormatUInt32(UInt32 value)
        {
            return string.Format("0x{0:X8}", value);
        }
    }
}
