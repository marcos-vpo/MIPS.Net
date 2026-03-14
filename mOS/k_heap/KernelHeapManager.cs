using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.k_objects;
using mOS.memory;

namespace mOS.k_heap
{
    internal class KernelHeapManager : ABIManaged
    {
        private const int HEAP_SIZE = 8192;
        private const int TUPLE_LEN = 8;
        private const int MAX_TUPLES = 1024;
        private readonly int BASE_ADDR = 0;

        private readonly PageAllocator pageAlloc;
        private readonly VirtualHeapManager virtualHeap;
        public KernelHeapManager(int baseAddrMap, PageAllocator pa)
        {
            BASE_ADDR = baseAddrMap;
            pageAlloc = pa;
            virtualHeap = new VirtualHeapManager();

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
                int order = BitConverter.ToInt32(tuple, 0);
                int pageIndex = BitConverter.ToInt32(tuple, 4);
                if (order == -1 && pageIndex == -1)
                    return addr;
            }
            // heap de entradas cheio
            return 0;
        }

        private List<HeapPage> _pages = new List<HeapPage>();
        // aloca uma nova página e grava no .space kernel_heap_map
        // de acordo com o retorno de FindFreeEntryAddr
        public int HeapAlloc(int size)
        {
            int[] physical_pages = new int[0];
            pageAlloc.ProtectedAllocate(size, out physical_pages);

            byte[] entryb = new byte[8];
            foreach (int page in physical_pages)
            {
                int emptyEntryAddr = FindFreeEntryAddr();

                int order = _pages.Count == 0 ? 0 : _pages.Max(e => e.Order) + 1;
                HeapPage entry = new HeapPage(emptyEntryAddr, order, page);

                Array.Copy(BitConverter.GetBytes(order), 0, entryb, 0, 4);
                Array.Copy(BitConverter.GetBytes(page), 0, entryb, 4, 4);

                m_write(emptyEntryAddr, ref entryb);

                _pages.Add(entry);
            }

            virtualHeap.UpdatePages(_pages);
            return physical_pages.Length;
        }

        public int WriteObject(mOSObject obj)
        {
            obj.Set(obj);
            byte[] objBin = obj.Serialize();
            int objTupleLen = 12 + objBin.Length;

            if (obj.VirtualAddr != -1) return virtualHeap.UpdateObj(obj, objBin);

            int virtualAddr = virtualHeap.FindFreeVirtualAddr(objTupleLen);

            if (virtualAddr == -1)
            {
                HeapAlloc(objTupleLen);
                virtualAddr = virtualHeap.FindFreeVirtualAddr(objTupleLen);
                if (virtualAddr == -1)
                    throw new Exception("Kernel full heap. TO-DO: correctly handle this! (future)");
            }

            virtualHeap.WriteObj(virtualAddr, obj, objBin);

            obj.SetVirtualAddr(virtualAddr);

            return virtualAddr;
        }

        internal T ReadObject<T>(int virtualHeapAddr) where T : mOSObject
        {
            const int HEADER_SIZE = 12;

            if (virtualHeapAddr < 0)
                return null;

            List<byte> fullPayload = new List<byte>();

            int currentAddr = virtualHeapAddr;
            short flags = 0;

            while (currentAddr != -1)
            {
                byte[] objHeader;
                byte[] objRaw;

                virtualHeap.ReadObject(currentAddr, out objHeader, out objRaw);

                // objeto inexistente ou free
                if (objRaw.Length == 0)
                    return null;

                // acumula payload
                fullPayload.AddRange(objRaw);

                // guarda flags do root
                if (currentAddr == virtualHeapAddr)
                    flags = BitConverter.ToInt16(objHeader[2..4]);

                // próximo da cadeia
                currentAddr = BitConverter.ToInt32(objHeader[4..8]);
            }

            // instancia o objeto lógico
            Type t = typeof(T);
            mOSObject obj = (mOSObject)Activator.CreateInstance(t);
            obj.Set(obj);

            obj.Desserialize(fullPayload.ToArray());

            obj.SetVirtualAddr(virtualHeapAddr);
            obj.SetFlags(flags);

            return (T)obj;
        }


        internal int FreeObject(int virtualHeapAddr)
        {
            return virtualHeap.FreeObj(virtualHeapAddr);
        }

        internal int ResolveVirtualToPhysical(int virtualAddr)
        {
            const int PAGE_SIZE = 4096;

            if (virtualAddr < 0 || virtualAddr >= virtualHeap.HeapLength)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr), "Endereço virtual fora do heap.");

            // determina em qual página virtual está
            int pageOrder = virtualAddr / PAGE_SIZE;
            int offsetInPage = virtualAddr % PAGE_SIZE;

            // encontra a página física correspondente
            HeapPage page = _pages.FirstOrDefault(p => p.Order == pageOrder);
            if (page == null)
                throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

            // endereço físico base da página
            int pagePhysicalAddr = PhysicalPageManager.PAGES_START_ADDR + (page.PageIndex * PAGE_SIZE);

            // endereço físico final
            return pagePhysicalAddr + offsetInPage;
        }

    }
}
