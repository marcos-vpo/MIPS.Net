using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOSLib.functions;

namespace mOSLib.heap
{
    internal class VirtualHeapManager : ABIManaged
    {
        private readonly sys sys;

        public VirtualHeapManager(sys sys)
        {
            this.sys = sys;
        }

        private List<VirtualHeapFragment> _fragments = new List<VirtualHeapFragment>();
        private List<VirtualHeapObject> _objects = new List<VirtualHeapObject>();
        private List<VirtualHeapPage> _pages = new List<VirtualHeapPage>();

        public void UpdatePages(List<VirtualHeapPage> toUpdate)
        {
            _pages = toUpdate.OrderBy(p => p.Order).ToList();
            int newHeapLen = _pages.Count * 4096;

            if (newHeapLen > HeapLength)
            {
                int hl = (newHeapLen - HeapLength);
                _fragments.Add(new VirtualHeapFragment(HeapLength, hl));
                _fragments = _fragments.OrderBy(f => f.VirtualHeapStart).ToList();
            }

            HeapLength = newHeapLen;

        }

        public int HeapLength { get; private set; }
        public int FindFreeVirtualAddr(int needed)
        {
            if (needed <= 0)
                throw new ArgumentOutOfRangeException(nameof(needed));

            foreach (var frag in _fragments)
            {
                if (frag.Length >= needed)
                    return frag.VirtualHeapStart;
            }

            return -1;
        }


        public void WriteObj(int virtualAddr, mOSObject obj, byte[] objBin)
        {


            int objTupleLen = 12 + objBin.Length;
            byte[] objTuple = new byte[objTupleLen];

            Array.Copy(BitConverter.GetBytes(obj.mOSType), 0, objTuple, 0, 2);
            Array.Copy(BitConverter.GetBytes((short)0), 0, objTuple, 2, 2); // flags
            Array.Copy(BitConverter.GetBytes(-1), 0, objTuple, 4, 4);      // continuation (virtual)
            Array.Copy(BitConverter.GetBytes(objBin.Length), 0, objTuple, 8, 4); // payload size
            Array.Copy(objBin, 0, objTuple, 12, objBin.Length);

            Write(virtualAddr, ref objTuple);

            VirtualHeapObject hpObj = new VirtualHeapObject
            {
                VirtualHeapAddr = virtualAddr,
                Type = obj.mOSType,
                Flags = 0,
                HObjSize = objBin.Length,
                ContinuationAddr = -1,
            };

            _objects.Add(hpObj);
            _objects = _objects.OrderBy(o => o.VirtualHeapAddr).ToList();

            /*
            impacto aqui: o heapfragment deve começar ao fim de onde está
            os bytes do programa mex
            */


            VirtualHeapFragment? fg = _fragments.FirstOrDefault(f => f.VirtualHeapStart == virtualAddr);
            if (fg != null)
            {
                var fragSize = fg.Length;
                if (objTupleLen == fragSize) _fragments.Remove(fg);
                else if (objTupleLen < fragSize)
                {
                    fg.VirtualHeapStart += objTupleLen;
                    fg.Length -= objTupleLen;
                }
            }

        }

        private void Read(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;
            Array.Clear(data);
            int pagesStartAddr = GetPageStartAddr();

            if (virtualAddr < 0 || virtualAddr + data.Length > HeapLength)
                return;


            int remaining = data.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                VirtualHeapPage page = _pages.FirstOrDefault(p => p.Order == pageOrder);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                int pagePhysicalAddr =
                    pagesStartAddr + ((page.VirtualPage - 1) * PAGE_SIZE);

                int bytesInThisPage =
                    Math.Min(remaining, PAGE_SIZE - offsetInPage);

                int physicalAddr =
                    pagePhysicalAddr + offsetInPage;

                byte[] temp = new byte[bytesInThisPage];
                m_read(physicalAddr + written, ref temp);

                Array.Copy(temp, 0, data, written, temp.Length);

                written += temp.Length;
                remaining -= temp.Length;
            }
        }

        private int page_start_addr = 0;
        private int GetPageStartAddr()
        {
            if (page_start_addr == 0)
                page_start_addr = sys.syscall(v0: call_codes.MEM_PAGE_START);
            return sys.syscall(v0: call_codes.MEM_PAGE_START);
        }

        private void Write(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;

            int pagesStartAddr = GetPageStartAddr();

            if (virtualAddr < 0 || virtualAddr + data.Length > HeapLength)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = data.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                // 1️⃣ Página virtual atual
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                // 2️⃣ Página mapeada
                VirtualHeapPage page = _pages.FirstOrDefault(p => p.Order == pageOrder);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                // 3️⃣ Endereço físico base da página
                int pagePhysicalAddr =
                    pagesStartAddr + ((page.VirtualPage - 1) * PAGE_SIZE);

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
                m_write(physicalAddr + written, ref slice);

                // 8️⃣ Avança
                written += bytesInThisPage;
                remaining -= bytesInThisPage;
                currentVirtualAddr += bytesInThisPage;
            }
        }

        byte[] vh_obj_header = new byte[12];
        internal void ReadObject(int virtualHeapAddr, out byte[] objHeader, out byte[] objRaw)
        {
            // [ 2 + 2 + 4 + 4 + N... ] = 
            Read(virtualHeapAddr, ref vh_obj_header);

            objHeader = vh_obj_header;

            short type = BitConverter.ToInt16(vh_obj_header[0..2]);
            short flags = BitConverter.ToInt16(vh_obj_header[2..4]);
            int continuationAddr = BitConverter.ToInt16(vh_obj_header[4..8]);
            int objSize = BitConverter.ToInt16(vh_obj_header[8..12]);

            objRaw = new byte[objSize];
            Read(virtualHeapAddr + 12, ref objRaw);
        }

        internal int FreeObj(int virtualHeapAddr)
        {
            int totalFreed = 0;
            int currentAddr = virtualHeapAddr;

            // 1️⃣ Transforma a cadeia de objetos em fragmentos livres e zera a memória
            while (currentAddr != -1)
            {
                Read(currentAddr, ref vh_obj_header);

                int len = BitConverter.ToInt32(vh_obj_header[8..12]);
                if (len <= 0) break;

                int continuationAddr = BitConverter.ToInt32(vh_obj_header[4..8]);
                int blockSize = 12 + len;

                VirtualHeapObject? ho = _objects.FirstOrDefault(o => o.VirtualHeapAddr == currentAddr);
                int payloadSize = ho != null ? ho.HObjSize : len;

                byte[] free = new byte[12 + payloadSize];
                Write(currentAddr, ref free);

                if (ho != null) _objects.Remove(ho);

                _fragments.Add(new VirtualHeapFragment(currentAddr, free.Length));
                totalFreed += blockSize;
                currentAddr = continuationAddr;
            }

            // 2️⃣ Consolida a tabela de fragmentos (Funde os vizinhos contíguos)
            ConsolidateFragments();

            // 3️⃣ Decommit de Páginas: Analisa o Heap e devolve páginas vazias ao Kernel (Exceto a Página 0)
            ReleaseEmptyPages(ref totalFreed);

            return totalFreed;
        }

        private void ConsolidateFragments()
        {
            _fragments = _fragments.OrderBy(f => f.VirtualHeapStart).ToList();

            for (int i = 0; i < _fragments.Count - 1;)
            {
                VirtualHeapFragment current = _fragments[i];
                VirtualHeapFragment next = _fragments[i + 1];

                if (current.VirtualHeapStart + current.Length == next.VirtualHeapStart)
                {
                    if (current.VirtualHeapStart > 0)
                        current.Length += next.Length;
                    _fragments.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
        }

        private void ReleaseEmptyPages(ref int totalFreed)
        {
            const int PAGE_SIZE = 4096;
            List<VirtualHeapPage> pagesToRelease = new List<VirtualHeapPage>();

            // 1️⃣ Descobre quais páginas (ignorando a página 0) estão totalmente livres
            // Uma página está livre se existe um fragmento que cobre todo o seu intervalo virtual
            foreach (var page in _pages)
            {
                if (page.Order == 0) continue; // 🚨 PROTEÇÃO CLÍNICA: Página zero nunca morre

                int pageStart = page.Order * PAGE_SIZE;
                int pageEnd = pageStart + PAGE_SIZE;

                // Procura um fragmento consolidado que engula totalmente esta página
                bool pageIsEmpty = _fragments.Any(f => pageStart >= f.VirtualHeapStart && (f.VirtualHeapStart + f.Length) <= pageEnd);

                if (pageIsEmpty)
                {
                    pagesToRelease.Add(page);
                }
            }

            if (pagesToRelease.Count == 0) return;

            // 2️⃣ Avisa o Kernel e limpa a tabela de páginas do processo
            foreach (var page in pagesToRelease)
            {
                sys.syscall(v0: call_codes.FREE, a2: physProcessAddr, a3: page.PhysicalPage);
                Thread.Sleep(50);
                sys.syscall(v0: call_codes.REM_MMU_TLB, a0: physProcessAddr, a1: page.PhysicalPage);

                _pages.Remove(page);
                totalFreed += 4096;
            }

            // 3️⃣ RECONSTRUÇÃO PURA DOS FRAGMENTOS
            // Em vez de fazer remendos matemáticos complexos em rebarbas, nós geramos a nova lista
            // de fragmentos livres subtraindo os espaços das páginas que acabaram de ser devolvidas ao Kernel.
            List<VirtualHeapFragment> newFragments = new List<VirtualHeapFragment>();

            foreach (var frag in _fragments)
            {
                int fragStart = frag.VirtualHeapStart;
                int fragEnd = frag.VirtualHeapStart + frag.Length;

                int currentStart = fragStart;

                // Passa cortando os pedaços que pertenciam às páginas liberadas
                foreach (var page in pagesToRelease.OrderBy(p => p.Order))
                {
                    int pageStart = page.Order * PAGE_SIZE;
                    int pageEnd = pageStart + PAGE_SIZE;

                    // Se a página intersecta o fragmento atual
                    if (pageStart >= currentStart && pageStart < fragEnd)
                    {
                        // Se sobrou um pedaço antes da página começar, guarda como fragmento válido
                        if (pageStart > currentStart)
                        {
                            newFragments.Add(new VirtualHeapFragment(currentStart, pageStart - currentStart));
                        }
                        currentStart = pageEnd; // Avança o ponteiro de análise para depois da página limpa
                    }
                }

                // Se sobrou alguma rebarba depois da última página limpa, adiciona
                if (currentStart < fragEnd)
                {
                    newFragments.Add(new VirtualHeapFragment(currentStart, fragEnd - currentStart));
                }
            }

            _fragments = newFragments;

            // 4️⃣ Sincroniza o tamanho do Heap e a ordem interna das páginas restantes
            UpdatePages(_pages);
        }

        internal int UpdateObj(mOSObject obj, byte[] objNewBin)
        {
            // 1️⃣ Localiza o objeto raiz
            VirtualHeapObject? root = _objects.FirstOrDefault(o => o.VirtualHeapAddr == obj.VirtualAddr);
            if (root == null)
                return 0;

            // 2️⃣ Coleta todas as partes (root + continuations)
            List<VirtualHeapObject> parts = new List<VirtualHeapObject>();
            VirtualHeapObject current = root;
            parts.Add(current);

            while (current.ContinuationAddr != -1)
            {
                current = _objects.FirstOrDefault(o => o.VirtualHeapAddr == current.ContinuationAddr)
                    ?? throw new Exception("Kernel heap corrupted (broken continuation chain)");

                parts.Add(current);
            }

            int currentTotalSize = parts.Sum(p => p.HObjSize);
            int newSize = objNewBin.Length;

            // ===============================
            // 🔴 CASO 1: NOVO <= ATUAL
            // ===============================
            if (newSize < currentTotalSize)
            {
                // libera tudo
                FreeObj(obj.VirtualAddr);

                WriteObj(obj.VirtualAddr, obj, objNewBin);
                return obj.VirtualAddr;
            }

            // ===============================
            // 🟢 CASO 2: NOVO > ATUAL
            // ===============================

            int written = 0;

            // 3️⃣ Reescreve payload nas partes existentes
            foreach (var part in parts)
            {
                int toWrite = Math.Min(part.HObjSize, newSize - written);
                if (toWrite <= 0)
                    break;

                byte[] slice = new byte[toWrite];
                Array.Copy(objNewBin, written, slice, 0, toWrite);

                Write(part.VirtualHeapAddr + 12, ref slice);
                written += toWrite;
            }

            // 4️⃣ Precisa de continuação nova?
            if (written < newSize)
            {
                int remaining = newSize - written;
                int continuationAddr = FindFreeVirtualAddr(remaining + 12);
                if (continuationAddr == -1)
                    throw new Exception("Out of virtual heap memory");

                // cria ProcessObject da continuação
                VirtualHeapObject newPart = new VirtualHeapObject
                {
                    VirtualHeapAddr = continuationAddr,
                    Type = obj.mOSType,
                    Flags = 1,
                    ContinuationAddr = -1,
                    HObjSize = remaining
                };

                // header
                Array.Copy(BitConverter.GetBytes(newPart.Type), 0, vh_obj_header, 0, 2);
                Array.Copy(BitConverter.GetBytes(newPart.Flags), 0, vh_obj_header, 2, 2);
                Array.Copy(BitConverter.GetBytes(-1), 0, vh_obj_header, 4, 4);
                Array.Copy(BitConverter.GetBytes(newPart.HObjSize), 0, vh_obj_header, 8, 4);

                byte[] remainingData = new byte[remaining];
                Array.Copy(objNewBin, written, remainingData, 0, remaining);

                Write(continuationAddr, ref vh_obj_header);
                Write(continuationAddr + 12, ref remainingData);

                // encadeia no ÚLTIMO bloco existente
                VirtualHeapObject last = parts.Last();
                last.ContinuationAddr = continuationAddr;

                byte[] contPtr = BitConverter.GetBytes(continuationAddr);
                Write(last.VirtualHeapAddr + 4, ref contPtr);

                _objects.Add(newPart);


                VirtualHeapFragment? fg = _fragments.FirstOrDefault(f => f.VirtualHeapStart == last.ContinuationAddr);
                if (fg != null)
                {
                    var fragSize = fg.Length;
                    if (remaining == fragSize) _fragments.Remove(fg);
                    else if (remaining < fragSize)
                    {
                        fg.VirtualHeapStart += 12 + remaining;
                        fg.Length -= 12 + remaining;
                    }
                }


                written += remaining;
            }

            return obj.VirtualAddr;
        }

        private int pId = -1;
        internal int GetSelfPId()
        {
            if (pId == -1)
                pId = sys.syscall(v0: 2);
            return pId;
        }

        private int physProcessAddr { get; set; }


        private List<VirtualHeapPage> _hpages = new List<VirtualHeapPage>();
        // aloca uma nova página e grava no .space kernel_heap_map
        // de acordo com o retorno de FindFreeEntryAddr
        public int HeapAlloc(int size)
        {
            int ret = HeapLength;
            sys.syscall(v0: call_codes.MALLOC, a2: size);

            byte[] return_data = new byte[64];
            m_read(0, ref return_data);

            List<int> validPages = new List<int>();
            for (int i = 0; i < return_data.Length; i += 4)
            {
                // Converte os 4 bytes atuais no número da página física correspondente
                int page = BitConverter.ToInt32(return_data, i);

                // REGRA CLÍNICA: Página 0 é inválida. Interrompe o fluxo imediatamente!
                if (page == 0)
                {
                    break;
                }

                validPages.Add(page);
            }

            // 3️⃣ Despeja o resultado final de volta no seu array original
            int[] physical_pages = validPages.ToArray();

            // pageAlloc.ProtectedAllocate(size, out physical_pages);
            int pId = GetSelfPId();
            int virtualPages_Indx = _pages.Count + 1;

            foreach (int physicalPage in physical_pages)
            {
                int order = _pages.Count == 0 ? 0 : _pages.Max(e => e.Order) + 1;
                VirtualHeapPage entry = new VirtualHeapPage(
                    processId: pId,
                    virtualOrder: order,
                    virtualPageIndex: virtualPages_Indx,
                    physicalPage: physicalPage,
                    PHeapUsage.DATA);

                syscall(v0: call_codes.ADD_MMU_TLB, a0: physProcessAddr, a1: physicalPage);

                _pages.Add(entry);
                virtualPages_Indx += 1;
            }

            UpdatePages(_pages);
            return ret;
        }

        internal void _init()
        {
            physProcessAddr = sys.syscall(v0: call_codes.PROGRAM_ADDR);
            pId = GetSelfPId();
            HeapAlloc(4096);

        }

        internal void attatch_pages(int[] physical_pages)
        {
            // pageAlloc.ProtectedAllocate(size, out physical_pages);
            int pId = GetSelfPId();
            int virtualPages_Indx = _pages.Count + 1;

            foreach (int physicalPage in physical_pages)
            {
                int order = _pages.Count == 0 ? 0 : _pages.Max(e => e.Order) + 1;
                VirtualHeapPage entry = new VirtualHeapPage(
                    processId: pId,
                    virtualOrder: order,
                    virtualPageIndex: virtualPages_Indx,
                    physicalPage: physicalPage,
                    PHeapUsage.DATA);

                syscall(v0: call_codes.ADD_MMU_TLB, a0: physProcessAddr, a1: physicalPage);

                _pages.Add(entry);
                virtualPages_Indx += 1;
            }

            UpdatePages(_pages);
        }

        internal void attatch_object(mOSObject obj)
        {
            if (_objects.Any(o => o.VirtualHeapAddr == obj.VirtualAddr) == false)
                _objects.Add(new VirtualHeapObject()
                {
                    VirtualHeapAddr = obj.VirtualAddr,
                    HObjSize = obj.Serialize().Length,
                    Flags = obj.Flags,
                    Type = obj.mOSType,
                    ContinuationAddr = -1
                });
        }
    }
}