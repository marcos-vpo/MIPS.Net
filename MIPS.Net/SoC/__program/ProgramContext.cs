using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MIPS.Net.Debugger;

namespace MIPS.Net.SoC.__program
{
    public class ProgramContext
    {
        public DebuggerTracingObject Tracing { get; set; }

        public int Address { get; private set; }
        public bool Loaded { get; private set; }
        public int Length { get; private set; }
        public byte Version { get; private set; }

        public int DataSectionSize { get; private set; }
        public DataEntry[] DataEntries { get; private set; }

        public int RotulesMapSectionSize { get; private set; }
        public RotuleMapping[] Rotules { get; private set; }
        internal Tuple<List<FfiAssembly>, List<FfiMethod>> LinkedAssemblies { get; private set; }
        public int Calling_RA { get; set; }

        public List<TLBEntry> tlb_entries = new List<TLBEntry>();
        public bool IsMMUEnabled  => tlb_entries.Count > 0; 

        internal void AddTLBEntry(int physical_page_index)
        {
            tlb_entries.Add(new TLBEntry(page: physical_page_index));
        }

        public void AttatchTracing(DebuggerTracingObject dto)
        {
            foreach (TracingRotule rtl in dto.Rotules)
            {
                int addr = Rotules[rtl.RotuleN].AbsoluteAddr;

                rtl.StartAddr = addr;
                foreach (RotuleInstruction rti in rtl.Instructions)
                {
                    rti.MemAddr = addr;
                    addr += 4;
                }
                rtl.EndAddr = addr - 4;
            }

            Tracing = dto;
        }

        public int GetCurrentRotule(int absoluteAddr)
        {
            for (int i = 0; i < Rotules.Length; i++)
            {
                var rt = Rotules[i];

                var start = rt.AbsoluteAddr;
                var end = (i == Rotules.Length - 1
                    ? rt.AbsoluteAddr + (absoluteAddr - rt.AbsoluteAddr)
                    : Rotules[i + 1].AbsoluteAddr);

                if (absoluteAddr >= start && absoluteAddr <= end)
                    return i;
            }
            return -1;
        }

        public TracingRotule? GetRotule(int rotule)
        {
            if (Tracing == null) return null;
            return Tracing.GetRotule(rotule);
        }

        public List<RotuleInstructionVM> GetRotuleInstructions(int rotuleN)
        {
            TracingRotule rtli = Tracing.Rotules.FirstOrDefault(r => r.RotuleN == rotuleN);
            if (rtli == null) return new List<RotuleInstructionVM>();

            List<RotuleInstructionVM> res = new List<RotuleInstructionVM>();
            foreach (RotuleInstruction ri in rtli.Instructions)
            {
                RotuleInstructionVM vm = new RotuleInstructionVM(ri.Code, ri.MemAddr, Tracing.SourceFile, ri);
                res.Add(vm);
            }
            return res;
        }

        public void DetatchTracing() => Tracing = null;

        public void Load(int programAddress)
        {
            Address = programAddress;
            if (IsMMUEnabled)
            {
                MMU.SetTLB(tlb_entries);
                programAddress = 64;
            }

            byte[] magic_byte = new byte[1];
            DMA.RequestData(programAddress, ref magic_byte);
            if (magic_byte[0] != 0xD7) throw new Exception("Invalid program. Byte [0] must be 0xD7, this will sinalyze a program start byte.");

 

        

            programAddress += 1;

            byte[] version = new byte[1];
            DMA.RequestData(programAddress, ref version);
            Version = version[0];
            programAddress += 1;

            byte[] totalLen = new byte[4];
            DMA.RequestData(programAddress, ref totalLen);
            Length = BitConverter.ToInt32(totalLen);
            programAddress += 4;

            using (ProgramReader pr = new ProgramReader(programAddress))
            {
                DataSectionSize = pr.ReadDataSectionSize();
                DataEntries = pr.ReadDataEntries(DataSectionSize);
                RotulesMapSectionSize = pr.ReadRotulesSectionSize();
                Rotules = pr.ReadRotulesMap(RotulesMapSectionSize);

                byte[] x = new byte[1];
                int rotulesArealen = (Rotules.Length * 4) + Rotules.Sum(r => r.RotuleLength);
                int endFileOrStartDotNet = pr.address + rotulesArealen;
                DMA.RequestData(endFileOrStartDotNet, ref x);
                if (x[0] == 0xDF) // program end
                    return;

                LinkedAssemblies = pr.ReadLinkedAssemblies(endFileOrStartDotNet);
                Loaded = true;
            }
        }

        internal DataEntry? GetDataEntryByRelativeAddr(int relativeAddr)
        {
            return DataEntries.FirstOrDefault(d => d.Index == relativeAddr);
        }

        internal RotuleMapping? GetRotuleByRelativeAddr(int intialRtl)
        {
            return Rotules[intialRtl];
        }

        private Registers _savedState = new Registers();
        internal void SaveState(Registers registers)
        {
            registers.CopyTo(_savedState);
        }

        internal void RestoreSavedSate(Registers registers)
        {
            _savedState.CopyTo(registers);
            _savedState.Reset();

            MMU.SetTLB(tlb_entries);
        }

        private List<Tuple<string, string, object>> ffi_objects = new List<Tuple<string, string, object>>();

        public ProgramContext(int addr = 0)
        {
            Address = addr;
            Loaded = false;
        }

        internal object? Get_FFI_Obj(string ffi_asm_name, string classNm)
        {
            Tuple<string, string, object>? obj = ffi_objects.FirstOrDefault(f =>
                f.Item1 == ffi_asm_name &&
                f.Item2 == classNm
            );

            if (obj == null) return null;

            return obj.Item3;
        }

        internal void Set_FFI_Obj(string ffi_asm_name, string classNm, object obj)
        {
            ffi_objects.Add(new Tuple<string, string, object>(ffi_asm_name, classNm, obj));
        }

        internal void Unload()
        {
            ffi_objects.Clear();

            if (LinkedAssemblies != null)
            {
                foreach (var o in LinkedAssemblies.Item1)
                {
                    try
                    {
                        if (o.Ctx != null)
                        {
                            o.Asm = null;
                            o.Ctx.Unload(); 
                            o.Ctx = null;
                        }
                    }
                    catch { }
                }
                LinkedAssemblies.Item1.Clear();
                LinkedAssemblies.Item2.Clear();
            }
            tlb_entries.Clear();
            MMU.SetTLB(tlb_entries);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public int FFIWaitUpAddr { get; private set; }
        internal void Set_FFI_WaitUpAddr(int waitUpAddress)
        {
            FFIWaitUpAddr = waitUpAddress;
        }
    }
}
