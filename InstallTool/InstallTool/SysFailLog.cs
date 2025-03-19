using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class SysFailLog
    {
        private ReceiveData mReceiveData;

        private enum SysFailLogEvtID : byte { FAULTEXCEPT = 0, ASSERTFAIL = 1};
        private enum FaultExceptType : UInt32 { NMI = 0,HARDFAULT, MEMMANAGEFAULT, BUSFAULT, USAGEFAULT, DEBUGMON };

        private struct Registers
        {
            public UInt32 R0, R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12, SP, LR, PC, xPSR;
        }

        private struct FaultExceptEvt
        {
            public Registers regs;
            public FaultExceptType type;
        }

        private struct AssertFailtEvt
        {
            public Registers regs;
        }

        public SysFailLog(AppComm appComm)
        {
            mReceiveData = new ReceiveData(appComm);
        }

        public bool receive()
        {
            byte[] sysFailLogData;
            bool bRet = mReceiveData.Receive(InstallToolDefs.ReceiveDataID.SYSFAILLOG, out sysFailLogData);

            parseSysFailLogData(sysFailLogData);

            return bRet;
        }

        private void showRegisters(Registers regs)
        {
            Console.WriteLine("R0 = 0x{0:X8}", regs.R0);
            Console.WriteLine("R1 = 0x{0:X8}", regs.R1);
            Console.WriteLine("R2 = 0x{0:X8}", regs.R2);
            Console.WriteLine("R3 = 0x{0:X8}", regs.R3);
            Console.WriteLine("R4 = 0x{0:X8}", regs.R4);
            Console.WriteLine("R5 = 0x{0:X8}", regs.R5);
            Console.WriteLine("R6 = 0x{0:X8}", regs.R6);
            Console.WriteLine("R7 = 0x{0:X8}", regs.R7);
            Console.WriteLine("R8 = 0x{0:X8}", regs.R8);
            Console.WriteLine("R9 = 0x{0:X8}", regs.R9);
            Console.WriteLine("R10 = 0x{0:X8}", regs.R10);
            Console.WriteLine("R11 = 0x{0:X8}", regs.R11);
            Console.WriteLine("R12 = 0x{0:X8}", regs.R12);
            Console.WriteLine("SP = 0x{0:X8}", regs.SP);
            Console.WriteLine("LR = 0x{0:X8}", regs.LR);
            Console.WriteLine("PC = 0x{0:X8}", regs.PC);
            Console.WriteLine("xPSR = 0x{0:X8}", regs.xPSR);

        }

        private void showAssertFailEvt(AssertFailtEvt evt)
        {
            Console.WriteLine("AssertFail event");
            showRegisters(evt.regs);
        }

        private void showExceptFailEvt(FaultExceptEvt evt)
        {
            Console.WriteLine("FaultExceptEvt event");
            showRegisters(evt.regs);
            Console.WriteLine("Type = {0}", evt.type);
        }

        private Registers parseRegisters(BinaryReader dataReader)
        {
            Registers regs;
            regs.R0 = dataReader.ReadUInt32();
            regs.R1 = dataReader.ReadUInt32();
            regs.R2 = dataReader.ReadUInt32();
            regs.R3 = dataReader.ReadUInt32();
            regs.R4 = dataReader.ReadUInt32();
            regs.R5 = dataReader.ReadUInt32();
            regs.R6 = dataReader.ReadUInt32();
            regs.R7 = dataReader.ReadUInt32();
            regs.R8 = dataReader.ReadUInt32();
            regs.R9 = dataReader.ReadUInt32();
            regs.R10 = dataReader.ReadUInt32();
            regs.R11 = dataReader.ReadUInt32();
            regs.R12 = dataReader.ReadUInt32();
            regs.SP = dataReader.ReadUInt32();
            regs.LR = dataReader.ReadUInt32();
            regs.PC = dataReader.ReadUInt32();
            regs.xPSR = dataReader.ReadUInt32();

            return regs;
        }

        private bool parseSysFailLogData(byte[] sysFailLogData)
        {
            using (var stream = new MemoryStream(sysFailLogData))
            {
                using (var dataReader = new BinaryReader(stream))
                {
                    while(dataReader.BaseStream.Position != dataReader.BaseStream.Length)
                    {
                        SysFailLogEvtID evtID = (SysFailLogEvtID)dataReader.ReadByte();
                        switch(evtID)
                        {
                            case SysFailLogEvtID.ASSERTFAIL:
                                AssertFailtEvt assertFailtEvt;
                                assertFailtEvt.regs = parseRegisters(dataReader);
                                showAssertFailEvt(assertFailtEvt);
                                break;
                            case SysFailLogEvtID.FAULTEXCEPT:
                                FaultExceptEvt faultExceptEvt;
                                faultExceptEvt.regs = parseRegisters(dataReader);
                                faultExceptEvt.type = (FaultExceptType)dataReader.ReadUInt32();
                                showExceptFailEvt(faultExceptEvt);
                                break;
                        }
                    }
                }
            }
            return true;
        }
    }
}
