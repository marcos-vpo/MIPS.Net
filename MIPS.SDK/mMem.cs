using MIPS.Abi;
using MIPS.Net.SoC;

namespace mOSLib
{
    public static class mMem
    {
        public static int alloc(int size)
        {
            return mSys.syscall(v0: CallCodes.MALLOC, a0: size);
        }
        public static int free(int addr)
        {
            return mSys.syscall(v0: CallCodes.FREE);
        }
        public static int realloc() { return 0; }
        public static int memset(int addr, byte b, int size) { return 0; }
        public static int memcpy(int destAddr, int srcAddr, int count) { return 0; }
        public static int memcmp(int addr1, int addr2, int count) { return 0; }

        private static int pagesStartAddr = 0;
        private static int total_heap_size = 0;

        public static void write(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;

            if (pagesStartAddr == 0)
                pagesStartAddr = mSys.syscall(v0: CallCodes.MEM_PAGE_START);
            if (total_heap_size == 0)
                total_heap_size = mSys.syscall(v0: CallCodes.MEM_TOTAL_HEAP);

            if (virtualAddr < 0 || virtualAddr + data.Length > total_heap_size)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = data.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                // 1️⃣ Página virtual atual
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                byte[] p_heap_page_tuple = new byte[16];
                DMA.RequestData(0, ref p_heap_page_tuple);

                // 2️⃣ Página mapeada
                ProcessHeapPage page = new ProcessHeapPage(ref p_heap_page_tuple);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

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
                Array.Copy(data, written, slice, 0, bytesInThisPage);

                // 7️⃣ Escrita via ABI
                DMA.StoreData(physicalAddr, ref slice);

                // 8️⃣ Avança
                written += bytesInThisPage;
                remaining -= bytesInThisPage;
                currentVirtualAddr += bytesInThisPage;
            }
        }

        public static void read(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;

            if (pagesStartAddr == 0)
                pagesStartAddr = mSys.syscall(v0: CallCodes.MEM_PAGE_START);
            if (total_heap_size == 0)
                total_heap_size = mSys.syscall(v0: CallCodes.MEM_TOTAL_HEAP);

            if (virtualAddr < 0 || virtualAddr + data.Length > total_heap_size)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = data.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                byte[] p_heap_page_tuple = new byte[16];
                DMA.RequestData(0, ref p_heap_page_tuple);

                ProcessHeapPage page = new ProcessHeapPage(ref p_heap_page_tuple);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                int pagePhysicalAddr =
                    pagesStartAddr + (page.PageIndex * PAGE_SIZE);

                int bytesInThisPage =
                    Math.Min(remaining, PAGE_SIZE - offsetInPage);

                int physicalAddr =
                    pagePhysicalAddr + offsetInPage;

                byte[] temp = new byte[bytesInThisPage];
                DMA.RequestData(physicalAddr, ref temp);

                Array.Copy(temp, 0, data, written, temp.Length);

                written += temp.Length;
                remaining -= temp.Length;
            }
        }

        static byte[] vh_obj_header = new byte[12];
    }
}
