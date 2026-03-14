using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using MIPS.Net.SoC;
using mOS.IOKit;
using mOS.IOKit.Devices;
using mOS.kernel;
using mOS.memory;
using mOS.process_heap;
using mOS.process_heap.ph_obj;
using mOS.process_table;

namespace mOS.process
{
    internal class ProccessManager : ABIManaged
    {
        private ProcessTableManager table;
        private ProcessHeapManager process_heap;
        private int current_pId;

        public ProccessManager(int process_table_map, int process_heap_map, PageAllocator page_alloc)
        {
            process_heap = new ProcessHeapManager(process_heap_map, page_alloc);
            table = new ProcessTableManager(process_table_map, page_alloc);
        }

        public void Launch(string path, string user)
        {
            string pname = path.Split('/').Last();
            try
            {
                using (IOStorageService stg = new IOStorageService())
                {
                    IOResult? res = stg.ReadAllBytes(path);

                    ProcessEntry pe = new ProcessEntry
                    {
                        UpTime = DateTime.Now.Ticks,
                        ProcessName = pname,
                        Status = ProcessStatus.RUNING,
                        StatusTime = DateTime.Now.Ticks,
                        User = user,
                        WorkingDirectory = path.Replace(pname, "")
                    };

                    table.WriteObject(pe);

                    int heapVirtualProgramAddr = process_heap.HeapAlloc(pId: pe.VirtualAddr, size: 64 + res.Data.Length, usage: PHeapUsage.CODE);
                    int programStartVirtualAddr = 64 + heapVirtualProgramAddr;
                    int physicalProgramAddr = process_heap.ResolveVirtualToPhysical(pId: pe.VirtualAddr, programStartVirtualAddr);

                    byte[] program_mex_asm = res.Data;

                    // write program code at 64 after 0 (in virtual memory of processId)
                    // first 64bytes will be used to stablish a 
                    // small data transfer channel with the kernel

                    process_heap.Write(pId: pe.VirtualAddr, virtualAddr: programStartVirtualAddr, ref program_mex_asm);


                    // pre load program
                    syscall(v0: 520, a0: physicalProgramAddr);

                    IReadOnlyCollection<ProcessHeapPage> processPages = process_heap.GetProcessPages(pId: pe.VirtualAddr);

                    ProgramParser pp = new ProgramParser(program_mex_asm, pages: processPages, programStartVirtualAddr, physicalProgramAddr);
                    //    if (pp.Rotules.Count < 4) return;

                    pe.MainAddr = pp.Rotules[0].PhysicalAddress;

                    table.WriteObject(pe); // update process entry

                    foreach (ProcessHeapPage page in processPages)
                    {

                        // add page as tlb entry
                        syscall(v0: 510, a0: physicalProgramAddr, a1: page.PageIndex);
                    }


                    k_loop.args(k_loo_action.go_to_process, physicalProgramAddr);

                    current_pId = pe.VirtualAddr;
                    p_list.Add(current_pId);
                }
            }
            catch (Exception e)
            {

            }
        }

        private List<int> p_list = new List<int>();

        internal int CurrentProcessId()
        {
            return current_pId;
        }

        List<Tuple<ProcessEntry, ProcessHeapPage[]>> _archiveds = new List<Tuple<ProcessEntry, ProcessHeapPage[]>>();
        internal void CurrentProgramExit()
        {
            List<ProcessHeapPage> code_pages = new List<ProcessHeapPage>();
            ProcessEntry pe = table.ReadObject<ProcessEntry>(current_pId);
            if (pe != null)
            {
                IReadOnlyCollection<ProcessHeapPage> pages = process_heap.GetProcessPages(current_pId);

                foreach (ProcessHeapPage proccess_page in pages)
                {
                    if (proccess_page.Usage == PHeapUsage.CODE)
                    {
                        process_heap.Detach(proccess_page);
                        code_pages.Add(proccess_page);
                    }
                    else
                    {
                        process_heap.Free(proccess_page);
                    }
                }

                table.FreeObject(pe.VirtualAddr);

                p_list.Remove(current_pId);
                current_pId = -1;
                _archiveds.Add(new Tuple<ProcessEntry, ProcessHeapPage[]>(pe, code_pages.ToArray()));
            }
        }

        internal ProcessInfo[] AllProcesses()
        {
            List<ProcessInfo> ps = new List<ProcessInfo>();
            ProcessInfo pk = new ProcessInfo
            {
                pId = -1,
                memUsage = mos_kernel.PhysicalPageManager.GetInfo().Wired,
                UpTime = mos_kernel.Uptime,
                Status = 0,
                User = "mOS_sys",
                WorkingDirectory = "~",
                ProcessName = "mOS_kernel",
                MainAddr = k_init.kernel_start,
            };

            ps.Add(pk);

            foreach (int p_id in p_list)
            {
                ProcessEntry pe = table.ReadObject<ProcessEntry>(p_id);
                ProcessInfo pi = new ProcessInfo
                {
                    pId = pe.VirtualAddr,
                    memUsage = process_heap.GetProcessPages(pe.VirtualAddr).Count() * 4096,
                    UpTime = DateTime.Now.Ticks - pe.UpTime,
                    Status = (byte)pe.Status,
                    StatusTime = DateTime.Now.Ticks - pe.StatusTime,
                    User = pe.User,
                    WorkingDirectory = pe.WorkingDirectory,
                    ProcessName = pe.ProcessName,
                    MainAddr = pe.MainAddr,
                };
                ps.Add(pi);
            }

            return ps.ToArray();
        }
    }
}
