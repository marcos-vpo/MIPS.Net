using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MIPS.Abi;
using mOS.k_objects;
using mOS.memory;
using mOS.process_heap.ph_obj;

namespace mOS.process_heap
{
    internal class ProcessHeapManager : ABIManaged
    {
        private const int HEAP_SIZE = 8192;
        private const int TUPLE_LEN = 16;
        private const int MAX_TUPLES = 1024;
        private readonly int BASE_ADDR = 0;
        const int PAGE_SIZE = 4096;

        private readonly PageAllocator pageAlloc;
        public ProcessHeapManager(int baseAddrMap, PageAllocator pa)
        {
            BASE_ADDR = baseAddrMap;
            pageAlloc = pa;

            int hp_addr = BASE_ADDR;
            byte[] empty = new byte[8];

            Array.Copy(BitConverter.GetBytes((int)-1), 0, empty, 0, 4);
            Array.Copy(BitConverter.GetBytes((int)-1), 0, empty, 4, 4);

            for (int i = 0; i < 1024; i++)
            {
                m_write(hp_addr, ref empty);
                hp_addr += 4;
            }

        }

        // obter endereco dentro do .space kernel_heap_map
        // no multiplo de 8, que esteja disponivel para gravar
        // uma ENTRADA DE PAGINA no heap
        private int FindFreeEntryAddr()
        {
            byte[] tuple = new byte[TUPLE_LEN];
            int end = BASE_ADDR + HEAP_SIZE;
            for (int addr = BASE_ADDR; addr < end; addr += TUPLE_LEN)
            {
                m_read(addr, ref tuple);
                int pId = BitConverter.ToInt32(tuple, 0);
                int order = BitConverter.ToInt32(tuple, 4);
                int pageIndex = BitConverter.ToInt32(tuple, 8);
                int usage = BitConverter.ToInt32(tuple, 12);

                if (order == -1 && pageIndex == -1)
                    return addr;
            }
            // heap de entradas cheio
            return 0;
        }

        private List<ProcessHeapPage> _pages = new List<ProcessHeapPage>();
        // aloca uma nova página e grava no .space kernel_heap_map
        // de acordo com o retorno de FindFreeEntryAddr
        public int HeapAlloc(int pId, int size, int usage)
        {
            int[] physical_pages = new int[0];
            pageAlloc.Allocate(size, out physical_pages);

            int addrResult = ProcessHeapLength(pId);

            byte[] entryb = new byte[16];

            int baseVirtualAddr = -1;

            foreach (int page in physical_pages)
            {
                int emptyEntryAddr = FindFreeEntryAddr();

                // guarda o PRIMEIRO endereço como base do heap
                if (baseVirtualAddr == -1)
                    baseVirtualAddr = emptyEntryAddr;

                int order = _pages.Count(p => p.ProcessId == pId) == 0
                    ? 0
                    : _pages.Where(p => p.ProcessId == pId).Max(e => e.Order) + 1;

                ProcessHeapPage entry =
                    new ProcessHeapPage(emptyEntryAddr, pId, order, page, usage);

                Array.Copy(BitConverter.GetBytes(pId), 0, entryb, 0, 4);
                Array.Copy(BitConverter.GetBytes(order), 0, entryb, 4, 4);
                Array.Copy(BitConverter.GetBytes(page), 0, entryb, 8, 4);
                Array.Copy(BitConverter.GetBytes(usage), 0, entryb, 12, 4);

                m_write(emptyEntryAddr, ref entryb);

                _pages.Add(entry);
            }

            return addrResult;
        }

        internal void Detach(ProcessHeapPage proccess_page)
        {
            byte[] entryb = new byte[16];
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 0, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 4, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 8, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 12, 4);
            m_write(proccess_page.EntryAddr, ref entryb);

            _pages.Remove(proccess_page);

            pageAlloc.MoveToInactive(proccess_page.PageIndex);
        }

        internal void Free(ProcessHeapPage proccess_page)
        {
            byte[] entryb = new byte[16];
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 0, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 4, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 8, 4);
            Array.Copy(BitConverter.GetBytes(-1), 0, entryb, 12, 4);
            m_write(proccess_page.EntryAddr, ref entryb);

            _pages.Remove(proccess_page);

            pageAlloc.MoveToFree(proccess_page.PageIndex);
        }

        public IReadOnlyCollection<ProcessHeapPage> GetProcessPages(int pId)
        {
            return _pages.Where(p => p.ProcessId == pId).OrderBy(p => p.Order).ToList();
        }

        internal int ResolveVirtualToPhysical(int pId, int virtualAddr)
        {
            const int PAGE_SIZE = 4096;

            int vheapLen = _pages.Where(p => p.ProcessId == pId).Count() * PAGE_SIZE;

            if (virtualAddr < 0 || virtualAddr >= vheapLen)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr), "Endereço virtual fora do heap.");

            // determina em qual página virtual está
            int pageOrder = virtualAddr / PAGE_SIZE;
            int offsetInPage = virtualAddr % PAGE_SIZE;

            // encontra a página física correspondente
            ProcessHeapPage page = _pages.FirstOrDefault(p => p.ProcessId == pId && p.Order == pageOrder);
            if (page == null)
                throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

            // endereço físico base da página
            int pagePhysicalAddr = PhysicalPageManager.PAGES_START_ADDR + (page.PageIndex * PAGE_SIZE);

            // endereço físico final
            return pagePhysicalAddr + offsetInPage;
        }

        public int ProcessHeapLength(int pId)
        {
            return _pages.Where(p => p.ProcessId == pId).Count() * PAGE_SIZE;
        }

        public void Read(int pId, int virtualAddr, ref byte[] outPut)
        {
            int pagesStartAddr = PhysicalPageManager.PAGES_START_ADDR;

            int pHeapLen = ProcessHeapLength(pId);

            if (virtualAddr < 0 || virtualAddr + outPut.Length > pHeapLen)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = outPut.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                ProcessHeapPage? page = _pages.FirstOrDefault(p => p.ProcessId == pId && p.Order == pageOrder);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                pageAlloc.UpdateLastAccess(page.PageIndex);

                int pagePhysicalAddr =
                    pagesStartAddr + (page.PageIndex * PAGE_SIZE);

                int bytesInThisPage =
                    Math.Min(remaining, PAGE_SIZE - offsetInPage);

                int physicalAddr =
                    pagePhysicalAddr + offsetInPage;

                byte[] temp = new byte[bytesInThisPage];
                m_read(physicalAddr, ref temp);

                Array.Copy(temp, 0, outPut, written, temp.Length);

                written += temp.Length;
                remaining -= temp.Length;
            }
        }


        public void Write(int pId, int virtualAddr, ref byte[] input)
        {
            int pagesStartAddr = PhysicalPageManager.PAGES_START_ADDR;

            int pHeapLen = ProcessHeapLength(pId);

            if (virtualAddr < 0 || virtualAddr + input.Length > pHeapLen)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = input.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                // 1️⃣ Página virtual atual
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                // 2️⃣ Página mapeada
                ProcessHeapPage? page = _pages.FirstOrDefault(p => p.ProcessId == pId && p.Order == pageOrder);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                pageAlloc.UpdateLastAccess(page.PageIndex);
                
                // 3️⃣ Endereço físico base da página
                int pagePhysicalAddr =
                    pagesStartAddr + (page.PageIndex * PAGE_SIZE);

                // 4️⃣ Quantos bytes cabem nesta página
                int bytesInThisPage =
                    Math.Min(remaining, PAGE_SIZE - offsetInPage);

                // 5️⃣ Endereço físico final
                int physicalAddr =
                    pagePhysicalAddr + offsetInPage;

                // 6️⃣ Fatia do buffer a gravar
                byte[] slice = new byte[bytesInThisPage];
                Array.Copy(input, written, slice, 0, bytesInThisPage);

                // 7️⃣ Escrita via ABI
                m_write(physicalAddr, ref slice);

                // 8️⃣ Avança
                written += bytesInThisPage;
                remaining -= bytesInThisPage;
                currentVirtualAddr += bytesInThisPage;
            }
        }

       
    }
}
