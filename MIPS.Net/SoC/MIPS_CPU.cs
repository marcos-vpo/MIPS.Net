using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.Debugger;
using MIPS.Net.InstructionSet;
using MIPS.Net.SoC.__program;

namespace MIPS.Net.SoC
{
    public sealed class MIPS_CPU
    {
        public static IClockCapture Capture { get; set; }
        public static MIPS_CPU Instance { get; private set; }


        public MMU MMU { get; private set; }
        public Registers Registers { get; private set; }
        public bool FFIEnabled { get; private set; }
        public int FFIWaitUpAddr => CurrentProgram == null ? 0 : CurrentProgram.FFIWaitUpAddr;

        public MIPS_CPU(DebuggerBridge? debugger)
        {
            DBG = debugger;
            Registers = new Registers();
            MMU = new MMU();
            Instance = this;
        }

        private void ProcessInstruction(byte[] instruction_unit)
        {
            int instruction = BitConverter.ToInt32(instruction_unit);
            if (instruction == 0)
            {
                if (pendingInterruption != null)
                {
                    if (Registers[$"k1"] == 0)
                    {
                        pendingInterruption.End();

                        pendingInterruption = null;
                        RestoreState();
                    }
                    else
                    {
                        pendingInterruption.End();
                        pendingInterruption = null;
                        _before_intr_program = 0;
                        _state_saved = false;

                        Registers["$pc"] = Registers["$k1"] - 4;
                        Registers["$k1"] = 0;
                        ProgramSwitch();
                    }
                    return;
                }
                else
                {
                    if (in_lock == false)
                    {
                        Console.Write("END OF PROGRAM");
                        _halted = true;
                    }
                    return;

                }
            }
            InstructionDecoder.DecodeAndExec(instruction, Registers);
        }

        private bool in_lock = false;
        private bool lock_request = false;
        public static void RequestLock()
        {
            Instance.lock_request = true;
            while (Instance.in_lock == false)
                Thread.Sleep(100);
        }

        public static bool InLock() => Instance.in_lock;

        public static void Release()
        {
            Instance.lock_request = false;
            Instance.in_lock = false;
        }

        public void Process(byte[] instructions)
        {

            while (in_lock)
            {
                if (_halted) return;
                Thread.Sleep(1);
            }

            lock_request = true;

            if (Registers["$k0"] == 1)
            {
                if (pendingInterruption != null)
                {
                    //        ProcessInstruction(new byte[4]);
                    //    pendingInterruption.End();
                    //   pendingInterruption = null;
                    //    Registers["$k0"] = 0;
                    //   _before_intr_program = Registers["$k1"];

                }
            }

            int lckWait = 0;
            while (!in_lock)
            {
                if (lckWait == 50 || Registers["$k1"] > 0) break;
                if (_halted) return;
                Thread.Sleep(1);
                lckWait += 1;
            }
            int pos = 0;
            byte[] instruction_unit = new byte[4];

            //     Registers rBackup = new Registers();
            //    Registers.CopyTo(rBackup);
            var pc = Registers["$k1"] == 0 ? Registers["$pc"] : Registers["$k1"];
            while (pos < instructions.Length)
            {
                Array.Copy(instructions, pos, instruction_unit, 0, 4);

                ProcessInstruction(instruction_unit);
                //     Thread.Sleep(2);
                int instruction = BitConverter.ToInt32(instruction_unit);
                if (instruction == 31)
                {
                    if (pendingInterruption != null)
                    {
                        if (Registers["k1"] > 0)
                            ProcessInstruction(new byte[4]);
                        else
                        {
                            int syscall_pos = pendingInterruption.HandlerAddress;
                            byte[] syscall_instr = new byte[4];

                            DMA.RequestData(syscall_pos, ref syscall_instr, true);

                            while (syscall_instr.Sum(x => x) != 0)
                            {
                                ProcessInstruction(syscall_instr);
                                //     Thread.Sleep(2);
                                if(in_lock == false)
                                {
                                    if (Registers["k0"] == 1)
                                        ProcessInstruction(new byte[4]);
                                   return;
                                }
                                syscall_pos += 4;
                                DMA.RequestData(syscall_pos, ref syscall_instr, true);
                            }

                            ProcessInstruction(syscall_instr); // end, exit interruption, restore state
                        }                            //        Thread.Sleep(2);                               //    pendingInterruption.End();

                        pendingInterruption = null;
                    }
                    else
                    {
                        if (Registers["k0"] == 1)
                            ProcessInstruction(new byte[4]);
                    }
                    // syscall
                }

                pos += 4;
            }

            //if (Registers["k1"] > 0)
            //{
            //    pc = Registers["k1"];
            //    Registers["k1"] = 0;
            //    _before_intr_program = 0;
            //}

            Registers["$pc"] = pc;
            Registers["$k1"] = 0;
            Registers["$k0"] = 0;
            //    rBackup.CopyTo(Registers);
            lock_request = false;
            in_lock = false;
        }

        private bool _halted { get; set; }
        public void RequestHalt()
        {
            _halted = true;
        }

        List<ProgramContext> programs = new List<ProgramContext>();
        internal static ProgramContext? CurrentProgram { get; private set; }
        short clock_status = 0;
        int c = 0;

        public DebuggerBridge DBG { get; set; }

        public static double Frequency { get; private set; }
        public static int ClockInterval { get; set; }

        double[] avg = new double[256];
        System.Timers.Timer tMon = new System.Timers.Timer();


        byte[] instruction_byte = new byte[4];
        int hertz = 0;

        CacheL1 L1 = new CacheL1();
        public void RunClock()
        {
            tMon.Interval = 1000;
            tMon.Elapsed += TMon_Elapsed;
            tMon.Start();
            pendingInterruption = null;
            Task.Run(() =>
            {
                // clock da CPU; busca instrução na memoria[i], executa, passa p/ proxima
                while (true)
                {
                    hertz += 1; // 1 hertz = 1 volta no loop
                    if (_halted)
                    {
                        MotherBoard.OnCPUHalted();

                        MMU.SetTLB(new List<TLBEntry>());
                        FFIEnabled = false;

                        L1.Clear();
                        Registers.Reset();
                        tMon.Stop();
                        tMon.Elapsed -= TMon_Elapsed;
                        foreach (var ctx in programs)
                        {
                            ctx.Unload();
                        }
                        CurrentProgram = null;
                        programs.Clear();
                        break;
                    }

                    if (clock_status == 0) clock_status = 1;
                    else if (clock_status == 1) clock_status = 0;

                    // enviar sinal de clock para a Motherboard
                    //       if (Capture != null)
                    //         try { Capture.MIPS_OnClock(clock_status, Frequency, ClockInterval); } catch { }

                    // há interrupções pendentes?
                    if (pendingInterruption != null)
                    {
                        // DBG?.Step(1);
                        if (_state_saved == false)
                        {
                            SaveState();


                            CurrentProgram = programs.FirstOrDefault(p => p.Rotules.Any(r => r.AbsoluteAddr == pendingInterruption.HandlerAddress));
                            Registers[Registers.PC] = pendingInterruption.HandlerAddress - 4;
                            ProgramSwitch();

                            Registers[Registers.PC] = pendingInterruption.HandlerAddress;

                            pendingInterruption.Ready = true;
                        }
                    }

                    if (DBG != null)
                    {
                        if (DBG.State == DebuggerState.PAUSED) continue;
                    }



                    if (in_lock)
                        continue;

                    if (lock_request)
                    {
                        in_lock = true;
                        continue;
                    }

                    // endereço instrução atual
                    int addr = Registers[Registers.PC];

                    Func<byte[], int> exec_instruction = new Func<byte[], int>((res) =>
                    {
                        if (addr != Registers[Registers.PC])
                            return 0;

                        if ((res == null) || (res.Length == 0))
                        {
                            DBG?.StateUpdate(CurrentProgram, 0, Registers, MotherBoard.Instance.Memory);
                            RequestHalt();
                            return 0;
                        }
                        byte[] instruction_unit = res;
                        if (instruction_unit[0] == 0xD7) // program starting, not loaded...
                        {
                            ProgramContext? pg = programs.FirstOrDefault(p => p.Address == addr);
                            if (pg == null) pg = new ProgramContext();
                            if (pg.Loaded == false)
                            {
                                if (pg.IsMMUEnabled)
                                    Instance.Registers[$"pc"] = 0;
                                pg.Load(addr);
                                programs.Add(pg);
                                CurrentProgram = pg;
                                CurrentProgram.Calling_RA = Registers[Registers.RA];

                                RotuleMapping? rtle = pg.GetRotuleByRelativeAddr(0);
                                if (rtle != null) Registers[Registers.PC] = rtle.AbsoluteAddr;
                                else Registers[Registers.PC] = pg.Rotules[0].AbsoluteAddr;

                                addr = Registers[Registers.PC];

                                DBG?.ProgramSwitching(pg, pc: addr);

                                MemoryBUS.SEND('R', addr, new byte[4], (KeyValuePair<bool, byte[]> res2) =>
                                {
                                    instruction_unit = res2.Value;
                                    ProcessInstruction(instruction_unit);
                                    if (!_halted) Registers[Registers.PC] += 4;
                                    StepOnDebug(instruction_unit);
                                    return 0;
                                });
                            }
                        }
                        else
                        {
                            ProcessInstruction(instruction_unit);
                            if (!_halted) Registers[Registers.PC] += 4;
                            StepOnDebug(instruction_unit);
                        }

                        return 0;
                    });

                    byte[]? cached = L1.GetInstruction(addr);
                    if (cached != null)
                    {
                        exec_instruction(cached);
                        //     MemoryBUS.State(addr, cached);
                    }
                    else
                    {
                        // fetch instruction
                        MemoryBUS.SEND('R', addr, instruction_byte, (KeyValuePair<bool, byte[]> res) =>
                        {
                            L1.StoreInstruction(addr, res.Value);
                            exec_instruction(res.Value);
                            return 0;
                        });
                    }

                    ///   D
                    if (ClockInterval > 0)
                        Thread.Sleep(ClockInterval);
                }
            });
        }

        public static void BUS_NOTIFICATION(bool enabled)
        {
            MemoryBUS.SetNotifications(enabled);
        }

        private void StepOnDebug(byte[] instruction_unit)
        {
            if (DBG == null) return;
            if (DBG.State == DebuggerState.DISCONNECTED) return;
            if (DBG != null)
            {
                int instruction = BitConverter.ToInt32(instruction_unit);
                if (DBG.State == DebuggerState.STEP)
                    DBG?.DoStep();
                if (DBG.State != DebuggerState.DISCONNECTED)
                    DBG?.StateUpdate(CurrentProgram, instruction, Registers, MotherBoard.Instance.Memory);
            }
        }

        private void TMon_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // Calcula a frequência
            Frequency = (double)hertz / 1000;
            hertz = 0;
        }


        InterruptionEntry pendingInterruption = null;
        private static object lckIntr = new object();
        internal void SendInterruption(InterruptionEntry interruption)
        {
            lock (lckIntr)
            {
                if (pendingInterruption == null)
                    pendingInterruption = interruption;
                else
                {
                    while (pendingInterruption.IsProcessing)
                    {
                        if (interruption.DeviceMemoryAddress > 0)
                        {
                            pendingInterruption = interruption;
                            Registers["pc"] = interruption.HandlerAddress - 4;
                            Release();
                            _state_saved = false;

                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);

                            if (pendingInterruption == null)
                            {
                                pendingInterruption = interruption;
                                break;
                            }
                        }

                        pendingInterruption = interruption;
                    }
                }
            }
        }


        private bool _state_saved = false;
        private int _before_intr_program = 0;
        internal void SaveState()
        {
            CurrentProgram?.SaveState(Registers);
            _before_intr_program = CurrentProgram.Address;
            //            Registers.Reset();
            CurrentProgram = null;
            _state_saved = true;

        }

        internal void RestoreState()
        {
            CurrentProgram = programs.FirstOrDefault(p => p.Address == _before_intr_program);
            if (CurrentProgram == null)
                throw new Exception($"CRITICAL: restore state of program in address '{_before_intr_program}' fail");
            CurrentProgram.RestoreSavedSate(Registers);

            Registers["$pc"] -= 4;
            _before_intr_program = 0;
            _state_saved = false;
        }

        internal static void ProgramSwitch()
        {
            var addr = Instance.Registers["$pc"] + 4;
            foreach (var pg in Instance.programs)
            {
                if (pg.Address == addr)
                {
                    CurrentProgram = pg;
                    if (pg.IsMMUEnabled)
                    {
                        MMU.SetTLB(pg.tlb_entries);
                        //             Instance.Registers[$"pc"] = -4;

                        if (pg.Loaded == false && pg.IsMMUEnabled)
                        {

                            pg.Load(addr);
                            CurrentProgram = pg;
                            CurrentProgram.Calling_RA = Instance.Registers[Registers.RA];
                            Instance.Registers[$"pc"] = pg.Rotules[0].AbsoluteAddr - 4;
                        }
                    }

                    MIPS_CPU.Instance.DBG?.ProgramSwitching(pg, addr);
                    return;
                }
                else if (pg.Loaded == false)
                {
                    continue;
                }

                var lastRtl = pg.Rotules.Last();

                for (int r = 0; r < pg.Rotules.Length; r++)
                {
                    var rtl = pg.Rotules[r];
                    if (addr == rtl.AbsoluteAddr || (addr > pg.Address && addr <= lastRtl.AbsoluteAddr))
                    {
                        CurrentProgram = pg;

                        MMU.SetTLB(pg.tlb_entries);
                        if (pg.IsMMUEnabled)
                        {

                            Instance.Registers["$pc"] = rtl.AbsoluteAddr - 4;
                        }

                        MIPS_CPU.Instance.DBG?.ProgramSwitching(pg, rtl.AbsoluteAddr);
                        return;
                    }
                }
            }
        }

        internal void EnableFFI(int waitUpAddress)
        {
            FFIEnabled = true;
            CurrentProgram?.Set_FFI_WaitUpAddr(waitUpAddress);

            //   FFIWaitUpAddr = waitUpAddress;
        }

        internal static Assembly FfiAtatchedDebug { get; private set; }
        public static void FFIDebug(Assembly asm)
        {
            FfiAtatchedDebug = asm;
        }

        public bool IsInterrupted()
        {
            return pendingInterruption != null;
        }

        internal static ProgramContext? GetProgram(int program_addr)
        {
            return Instance.programs.FirstOrDefault(p => p.Address == program_addr);
        }

        internal static void InitMMU(int page_area_start, int page_size)
        {
            Instance.MMU.Initialize(page_area_start, page_size);
        }

        internal static void AttatchPreloadedProgram(ProgramContext ctx)
        {
            Instance.programs.Add(ctx);
        }

        internal static void UnloadProgramCtx(int addr)
        {
            var program = GetProgram(addr);
            if (program != null)
            {
                program.Unload();
                Instance.programs.Remove(program);
                program = null;
            }
        }
    }
}
