using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC.__program;

namespace MIPS.Net.SoC
{
    public class InterruptionController
    {
        internal static InterruptionController Instance { get; private set; }
        private Registers _registers;
        public InterruptionController(Registers regs)
        {
            _registers = regs;
            Instance = this;
        }

        public InterruptionEntry[] GetInterruptions()
        {
            List<InterruptionEntry> _ = new List<InterruptionEntry>();

            byte[] interruption_table = new byte[9996]; // 833 entries max

            DMA.RequestData(0, ref interruption_table, noMMU: true);

            int entries_count = interruption_table.Length / 12;

            int indx = 0;


            byte[] tuple = new byte[12];

            byte[] int_code = new byte[4];
            byte[] handler_addr = new byte[4];
            byte[] device_addr = new byte[4];

            for (int i = 0; i < entries_count; i++)
            {
                Array.Copy(interruption_table, indx, tuple, 0, tuple.Length);

                Array.Copy(tuple, 0, int_code, 0, int_code.Length);
                Array.Copy(tuple, 4, handler_addr, 0, handler_addr.Length);
                Array.Copy(tuple, 8, device_addr, 0, device_addr.Length);

                int code = BitConverter.ToInt32(int_code);
                int handlerAddr = BitConverter.ToInt32(handler_addr);
                int deviceAddr = BitConverter.ToInt32(device_addr);

                _.Add(new InterruptionEntry
                {
                    InterruptionAddress = indx,
                    Code = code,
                    HandlerAddress = handlerAddr,
                    DeviceMemoryAddress = deviceAddr
                });
                indx += 12;
            }



            return _.ToArray();
        }

        public void CallInterruption(int code, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            if (code == 2)
            {

            }

            if (code == 101) HandleInitializeDevice(callBack);
            else if (code == 427) HandleStopDbg(callBack);
            else if (code == 102) HandleInitializeHardwareButton(callBack);
            else if (code == 110) HandlerBlockTransfer(callBack);
            else if (code == 115) HandlerBlockClear(callBack);
            else if (code == 210) HandleGetFreeInterruptionEntryAddress(callBack);
            else if (code == 211) HandleGetInterruptionAddrByCode(callBack);
            else if (code == 310) HandleSetCPUClockInterval(callBack);
            else if (code == 400) MIPS_CPU.Instance.DBG?.RequestDebugger();
            else if (code == 410) HandleEnableFFI(callBack);
            else if (code == 500) HandleInitMMU(callBack);
            else if (code == 510) HandleAddTLBMMU(callBack);
            else if (code == 520) HandlePreLoadProgramCtx(callBack);
            else if (code == 521) UnloadProgramCtx(callBack);

            else ExecuteFromInterruptionTable(code, callBack);
        }

        private void HandlePreLoadProgramCtx(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int addr = _registers["$a0"];
            byte[] pStart = new byte[1];
            DMA.RequestData(addr, ref pStart);

            if (pStart[0] == 0xD7)
            {
                ProgramContext ctx = new ProgramContext(addr);
                MIPS_CPU.AttatchPreloadedProgram(ctx);
            }


        }

        private void UnloadProgramCtx(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int addr = _registers["$a0"]; 
            byte[] pStart = new byte[1];
            DMA.RequestData(addr, ref pStart);

            if (pStart[0] == 0xD7)
            {
                MIPS_CPU.UnloadProgramCtx(addr);
            }
        }

        private void HandleAddTLBMMU(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int program_addr = _registers["$a0"];
            int phy_page = _registers["$a1"];

            ProgramContext? ctx = MIPS_CPU.GetProgram(program_addr);
            if (ctx != null)
            {
                ctx.AddTLBEntry(phy_page);
            }
        }

        private void HandleInitMMU(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int page_area_start = _registers["$a0"];
            int page_size = _registers["$a1"];

            MIPS_CPU.InitMMU(page_area_start, page_size);

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleEnableFFI(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int waitUpAddress = _registers["$a0"];
            MIPS_CPU.Instance.EnableFFI(waitUpAddress);
            //      callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleStopDbg(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleGetInterruptionAddrByCode(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int code = _registers["$a0"];
            var ints = GetInterruptions();
            var entry = ints.FirstOrDefault(i => i.Code == code);
            if (entry != null)
                _registers["$v0"] = entry.InterruptionAddress;

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleSetCPUClockInterval(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int interval = _registers["$a0"];
            MIPS_CPU.ClockInterval = interval;

            _registers["$v0"] = 1;
            _registers["$a0"] = 0;

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleGetFreeInterruptionEntryAddress(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            var ints = GetInterruptions();
            var intFreeEntry = ints.FirstOrDefault(i => i.Code == 0);
            if (intFreeEntry != null)
                _registers["$v0"] = intFreeEntry.InterruptionAddress;


            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleInitializeHardwareButton(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            byte devID = (byte)_registers["$a0"];
            int interruptionCodeClick = _registers["$a1"];

            var ints = GetInterruptions();
            var intr = ints.FirstOrDefault(i => i.Code == interruptionCodeClick);
            if (intr == null)
            {
                // ver como tratar
                return;
            }

            MotherBoard.InitializeHardwareButton(devID, interruptionCodeClick);

            _registers["$v0"] = 1;
            _registers["$a0"] = 0;
            _registers["$a1"] = 0;

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandlerBlockClear(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int sourceAddr = _registers["$a0"];
            int length = _registers["$a1"];
            byte[] data = new byte[length];
            DMA.StoreData(sourceAddr, ref data);

            _registers["$v0"] = 1;
            _registers["$a0"] = 0;
            _registers["$a1"] = 0;
            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandlerBlockTransfer(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int sourceAddr = _registers["$a0"];
            int destinationAddr = _registers["$a1"];
            int length = _registers["$a2"];

            if (destinationAddr > 10000)
            {
                byte[] data = new byte[length];
                DMA.RequestData(sourceAddr, ref data);
                DMA.StoreData(destinationAddr, ref data);
            }
            else
            {
                var ints = GetInterruptions().Length;

                if (ints == 0)
                {
                    byte[] data = new byte[length];
                    DMA.RequestData(sourceAddr, ref data);
                    DMA.StoreData(destinationAddr, ref data);
                }
            }
            _registers["$v0"] = 1;
            _registers["$a0"] = 0;
            _registers["$a1"] = 0;
            _registers["$a2"] = 0;

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void HandleInitializeDevice(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            int connPort = _registers["$a0"];
            byte devID = (byte)_registers["$a1"];
            int addr = _registers["$a2"];


            MotherBoard.InitializePort(devID, connPort, addr);
            _registers["$v0"] = 1;
            _registers["$a0"] = 0;
            _registers["$a1"] = 0;
            _registers["$a2"] = 0;

            if (callBack != null) callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
        }

        private void ExecuteFromInterruptionTable(int code, Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            InterruptionEntry[] ints = GetInterruptions();
            InterruptionEntry? intr = ints.FirstOrDefault(i => i.Code == code);

            if (intr == null)
            {
                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
                return;
            }

            if (intr.HandlerAddress == 0)
            {
                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
                return;
            }

            _registers[$"a0"] = code;
            _registers[$"a1"] = intr.DeviceMemoryAddress;

            MIPS_CPU.Instance.SendInterruption(intr);
            //  Task.Run(() => MIPS_CPU.Instance.SendInterruption(intr));


            bool sincExec = MIPS_CPU.Instance.Registers["$k0"] == 1;




            if (sincExec)
            {
                while (intr.Ready == false)
                    Thread.Sleep(10);

                intr.WaitInterruptionExecution();
                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
                return;
            }

            Task.Run(() =>
            {
                while (intr.Ready == false)
                    Thread.Sleep(10);
                //       while (intr.IsProcessing) Thread.Sleep(100);
                intr.WaitInterruptionExecution(true);

                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
            });
        }

        internal InterruptionEntry? GetInterruption(int intrCode)
        {
            return GetInterruptions().FirstOrDefault(i => i.Code == intrCode);
        }
    }
}
