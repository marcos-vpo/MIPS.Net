using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using MIPS.Net.SoC;
using mOS.kernel;

namespace mOS.memory
{
    internal class PhysicalPageManager : ABIManaged
    {
        public int START_MEM_ADDR { get; private set; }
        public int AVAILABLE { get; private set; }
        public int PAGES { get; private set; }
        public int TABLE_LEN { get; private set; }
        public static int PAGES_START_ADDR { get; private set; }

        private PhysicalPageEntry[] _entries;

        byte[] clean_buffer = new byte[0];
        internal void _INIT(int start_mem_addr)
        {
            calc_mem(start_mem_addr);

            const int PAGE_SIZE = 4096;
            const int ENTRY_SIZE = 16;

            clean_buffer = new byte[PAGE_SIZE];

            PAGES = AVAILABLE / (PAGE_SIZE + ENTRY_SIZE);
            TABLE_LEN = PAGES * ENTRY_SIZE;
            PAGES_START_ADDR = START_MEM_ADDR + TABLE_LEN;

            init_entries();


            int tmpBufferAddr = k_init.usb01_addr + 10;
            byte[] psAddrB = BitConverter.GetBytes(PAGES_START_ADDR);
            DMA.StoreData(tmpBufferAddr, ref psAddrB);
            /*
            asm($@"li $a0, {tmpBufferAddr}
lw $a0, 0($a0)");
            */
            byte[] tmpBfAddrB = new byte[4];
            DMA.RequestData(tmpBufferAddr, ref  tmpBfAddrB);
            int a0 = BitConverter.ToInt32(tmpBfAddrB);

            // init MMU
            syscall(v0: 500, a0: a0, a1: PAGE_SIZE);

        }


        private void init_entries()
        {
            _entries = new PhysicalPageEntry[PAGES];

            int pages_addr = PAGES_START_ADDR;
            int table_addr = START_MEM_ADDR;


            DateTime la = DateTime.Now;
            byte[] entry_b = new byte[16];
            byte[] page_clear = new byte[4096];
            int pageIndex = 0;
            for (int i = 0; i < PAGES; i++)
            {
                PhysicalPageEntry ppe = new PhysicalPageEntry(
                    entry_index: i,
                    entry_addr: table_addr,
                    page_index: pageIndex,
                    phys_addr: pages_addr,
                    status: PageStatus.FREE,
                    last_access: la.Ticks
                );

                Array.Copy(
                    sourceArray: BitConverter.GetBytes(ppe.PHYS_ADDR), 0, entry_b, 0, 4
                );

                Array.Copy(
                  sourceArray: new byte[1] { ppe.STATUS }, 0, entry_b, 4, 1
                );

                Array.Copy(
                 sourceArray: BitConverter.GetBytes(ppe.LAST_ACCESS), 0, entry_b, 8, 8
               );

                m_write(table_addr, ref entry_b);
                m_write(pages_addr, ref page_clear);


                table_addr += 16;
                pages_addr += 4096;
                pageIndex += 1;

                _entries[i] = ppe;
            }

        }

        private void calc_mem(int start_mem_addr)
        {
            START_MEM_ADDR = start_mem_addr;
            AVAILABLE = 0;

            byte[] b = new byte[1];
            int addr = start_mem_addr;

            while (true)
            {
                m_read(addr, ref b);
                if (b[0] == 0xD7) break;

                AVAILABLE++;
                addr++;
            }
        }

        private MemInfo _info;
        internal MemInfo GetInfo()
        {
            if (_info == null) _info = new MemInfo();

            _info.Update(
                totalMemory: AVAILABLE,
                active: 4096 * _entries.Where(p => p.STATUS == PageStatus.ACTIVE).Count(),
                inactive: 4096 * _entries.Where(p => p.STATUS == PageStatus.INACTIVE).Count(),
                wired: 4096 * _entries.Where(p => p.STATUS == PageStatus.WIRED).Count(),
                free: 4096 * _entries.Where(p => p.STATUS == PageStatus.FREE).Count()
            );

            return _info;
        }

        internal PhysicalPageEntry Page(int pageIndex)
        {
            return _entries[pageIndex];
        }

        internal int[] AllocFromFree(int bytes, bool wired = false)
        {
            List<PhysicalPageEntry> pages = _entries.Where(p => p.STATUS == PageStatus.FREE).ToList();
            return InternalAllocate(bytes, wired, pages);
        }

        internal int[] AllocFromInactive(int bytes, int inactiveTimeMinutes, bool wired = false)
        {
            var now = DateTime.Now;

            List<PhysicalPageEntry> pages = _entries.Where(p =>
                p.STATUS == PageStatus.INACTIVE &&
                (now - DateTime.FromBinary(p.LAST_ACCESS)).TotalMinutes > inactiveTimeMinutes
            ).ToList();
            return InternalAllocate(bytes, wired, pages);
        }

        private int[] InternalAllocate(int bytes, bool wired, List<PhysicalPageEntry> frees)
        {
            int allocated = 0;
            List<int> pages = new List<int>();
            for (int p = 0; p < frees.Count; p++)
            {
                PhysicalPageEntry pe = frees[p];
                pe.STATUS = wired ? PageStatus.WIRED : PageStatus.ACTIVE;
                pe.LAST_ACCESS = DateTime.Now.Ticks;

                _entries[pe.ENTRY_INDEX] = pe;

                byte[] b_status = new byte[1] { pe.STATUS };
                m_write(pe.ENTRY_ADDR + 4, ref b_status);

                byte[] b_last_access = BitConverter.GetBytes(pe.LAST_ACCESS);
                m_write(pe.ENTRY_ADDR + 8, ref b_last_access);

                pages.Add(pe.PageIndex);
                allocated += 4096;

                if (allocated >= bytes) break;
            }

            return pages.ToArray();
        }

        internal void UpdateLastAccess(int pageIndex)
        {
            var pe = Page(pageIndex);
            pe.LAST_ACCESS = DateTime.Now.Ticks;

            byte[] b_last_access = BitConverter.GetBytes(pe.LAST_ACCESS);
            m_write(pe.ENTRY_ADDR + 8, ref b_last_access);
        }

        internal void MoveToInactive(int pageIndex)
        {
            var pe = Page(pageIndex);
            pe.STATUS = PageStatus.INACTIVE;
            pe.LAST_ACCESS = DateTime.Now.Ticks;

            byte[] b_last_access = BitConverter.GetBytes(pe.LAST_ACCESS);
            m_write(pe.ENTRY_ADDR + 8, ref b_last_access);

            byte[] b_status = new byte[1] { pe.STATUS };
            m_write(pe.ENTRY_ADDR + 4, ref b_status);

            _entries[pageIndex] = pe;
        }

        internal void MoveToFree(int pageIndex)
        {
            var pe = Page(pageIndex);
            pe.STATUS = PageStatus.FREE;
            pe.LAST_ACCESS = DateTime.Now.Ticks;

            byte[] b_last_access = BitConverter.GetBytes(pe.LAST_ACCESS);
            m_write(pe.ENTRY_ADDR + 8, ref b_last_access);

            byte[] b_status = new byte[1] { pe.STATUS };
            m_write(pe.ENTRY_ADDR + 4, ref b_status);

            _entries[pageIndex] = pe;

            m_write(pe.PHYS_ADDR, ref clean_buffer);
        }
    }
}
