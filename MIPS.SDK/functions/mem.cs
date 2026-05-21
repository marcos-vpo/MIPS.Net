using MIPS.Abi;
using MIPS.Net.SoC;
using mOSLib.heap;

namespace mOSLib.functions
{
    internal class mem
    {
        private sys sys;
        private VirtualHeapManager virtualHeap = null;

        public mem(sys sys)
        {
            this.sys = sys;
            virtualHeap = new VirtualHeapManager(this.sys);
            virtualHeap._init();
        }
        public int alloc(int size)
        {
            return virtualHeap.HeapAlloc(size); //sys.syscall(v0: call_codes.MALLOC, a2: size);
        }

        public int free(int virtualHeapAddr)
        {
            return virtualHeap.FreeObj(virtualHeapAddr);
        }

        public int realloc() { return 0; }
        public int memset(int addr, byte b, int size) { return 0; }
        public int memcpy(int destAddr, int srcAddr, int count) { return 0; }
        public int memcmp(int addr1, int addr2, int count) { return 0; }

        private int pagesStartAddr = 0;
        private int total_heap_size = 0;

        public int write(mOSObject obj)
        {
            obj.Set(obj);
            byte[] objBin = obj.Serialize();
            int objTupleLen = 12 + objBin.Length;

            if (obj.VirtualAddr != -1) return virtualHeap.UpdateObj(obj, objBin);

            int virtualAddr = virtualHeap.FindFreeVirtualAddr(objTupleLen);

            if (virtualAddr == -1)
            {
                virtualHeap.HeapAlloc(objTupleLen);
                virtualAddr = virtualHeap.FindFreeVirtualAddr(objTupleLen);
                if (virtualAddr == -1)
                    throw new Exception("Process-heap are full. TO-DO: correctly handle this! (future)");
            }

            virtualHeap.WriteObj(virtualAddr, obj, objBin);

            obj.SetVirtualAddr(virtualAddr);

            return virtualAddr;
        }

        public T read<T>(int virtualHeapAddr) where T : mOSObject
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

            virtualHeap.attatch_object(obj);

            return (T)obj;
        }

        internal void attatch(List<int> validPages)
        {
            // 3️⃣ Despeja o resultado final de volta no seu array original
            int[] physical_pages = validPages.ToArray();

            virtualHeap.attatch_pages(physical_pages);
        }
    }
}
