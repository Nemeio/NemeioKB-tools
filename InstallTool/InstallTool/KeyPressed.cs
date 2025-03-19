using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class KeyPressed : AppCommCmd
    {
        private const InstallToolDefs.AppCommCmdID KeyPressedCmdId = InstallToolDefs.AppCommCmdID.KEYPRESSED;
        private const int DelayKeepAlive = 1000;

        public KeyPressed(AppComm appComm) : base(appComm, KeyPressedCmdId)
        {
        }

        public void test(int durationSec)
        {
            KeepAlive keepAlive = new KeepAlive(mAppComm);
            keepAlive.sendKeepAlive(durationSec * 1000 / DelayKeepAlive, DelayKeepAlive);
        }

        private List<Int32> parseKeyStrokes(byte[] data)
        {
            using (var keyData = new MemoryStream(data))
            {
                using (var binKeyData = new BinaryReader(keyData))
                {
                    List<Int32> keystrokes = new List<Int32>();

                    sbyte count = binKeyData.ReadSByte();
                    if (count <= 0)
                    {
                        return keystrokes;
                    }

                    int pDescriptor = 0;

                    do
                    {
                        byte[] action = binKeyData.ReadBytes(4);

                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(action);

                        keystrokes.Add(BitConverter.ToInt32(action, 0));

                        pDescriptor++;

                    } while (pDescriptor < count);

                    return keystrokes;
                }
            }
        }

        public override void CmdReceived(byte[] data)
        {
            List<Int32> keystrokes = parseKeyStrokes(data);

            if (keystrokes.Count != 0)
            {
                Console.Write("Pressed keys =");
                foreach (var keystroke in keystrokes)
                {
                    Console.Write(" " + keystroke);
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Released all keys");
            }            
        }
    }
}
