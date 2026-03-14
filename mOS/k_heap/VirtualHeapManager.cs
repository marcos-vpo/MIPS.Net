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
    internal class VirtualHeapManager : ABIManaged
    {

        private List<HeapFragment> _fragments = new List<HeapFragment>();
        private List<HeapObject> _objects = new List<HeapObject>();
        private List<HeapPage> _pages = new List<HeapPage>();

        public void UpdatePages(List<HeapPage> toUpdate)
        {
            _pages = toUpdate.OrderBy(p => p.Order).ToList();
            int newHeapLen = _pages.Count * 4096;

            if (newHeapLen > HeapLength)
            {
                int hl = (newHeapLen - HeapLength);
                _fragments.Add(new HeapFragment(HeapLength, hl));
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

            HeapObject hpObj = new HeapObject
            {
                VirtualHeapAddr = virtualAddr,
                Type = obj.mOSType,
                Flags = 0,
                HObjSize = objBin.Length,          // 🔴 AQUI está a correção real
                ContinuationAddr = -1,
            };

            _objects.Add(hpObj);
            _objects = _objects.OrderBy(o => o.VirtualHeapAddr).ToList();

            HeapFragment? fg = _fragments.FirstOrDefault(f => f.VirtualHeapStart == virtualAddr);
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



        public void Read(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;

            int pagesStartAddr = PhysicalPageManager.PAGES_START_ADDR;

            if (virtualAddr < 0 || virtualAddr + data.Length > HeapLength)
                throw new ArgumentOutOfRangeException(nameof(virtualAddr));

            int remaining = data.Length;
            int written = 0;
            int currentVirtualAddr = virtualAddr;

            while (remaining > 0)
            {
                int pageOrder = currentVirtualAddr / PAGE_SIZE;
                int offsetInPage = currentVirtualAddr % PAGE_SIZE;

                HeapPage page = _pages.FirstOrDefault(p => p.Order == pageOrder);
                if (page == null)
                    throw new InvalidOperationException($"Heap page {pageOrder} não mapeada");

                int pagePhysicalAddr =
                    pagesStartAddr + (page.PageIndex * PAGE_SIZE);

                int bytesInThisPage =
                    Math.Min(remaining, PAGE_SIZE - offsetInPage);

                int physicalAddr =
                    pagePhysicalAddr + offsetInPage;

                byte[] temp = new byte[bytesInThisPage];
                m_read(physicalAddr, ref temp);

                Array.Copy(temp, 0, data, written, temp.Length);

                written += temp.Length;
                remaining -= temp.Length;
            }
        }

        public void Write(int virtualAddr, ref byte[] data)
        {
            const int PAGE_SIZE = 4096;

            int pagesStartAddr = PhysicalPageManager.PAGES_START_ADDR;

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
                HeapPage page = _pages.FirstOrDefault(p => p.Order == pageOrder);
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
                m_write(physicalAddr, ref slice);

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

            while (currentAddr != -1)
            {
                // lê header do nó atual
                Read(currentAddr, ref vh_obj_header);

                int len = BitConverter.ToInt32(vh_obj_header[8..12]);
                if (len <= 0)
                    break; // já free ou corrupção

                int continuationAddr = BitConverter.ToInt32(vh_obj_header[4..8]);
                int blockSize = 12 + len;

                HeapObject? ho =
                    _objects.FirstOrDefault(o => o.VirtualHeapAddr == currentAddr);


                // zera memória do bloco
                byte[] free = new byte[12 + ho.HObjSize];
                Write(currentAddr, ref free);

                // remove da tabela de objetos
        
                if (ho != null)
                    _objects.Remove(ho);

                // cria fragmento
                _fragments.Add(new HeapFragment(currentAddr, free.Length));

                totalFreed += blockSize;

                // avança na cadeia
                currentAddr = continuationAddr;
            }

            // ordena fragmentos
            _fragments = _fragments
                .OrderBy(f => f.VirtualHeapStart)
                .ToList();

            // unifica fragmentos contíguos
            for (int i = 0; i < _fragments.Count - 1;)
            {
                HeapFragment current = _fragments[i];
                HeapFragment next = _fragments[i + 1];

                if (current.VirtualHeapStart + current.Length == next.VirtualHeapStart)
                {
                    current.Length += next.Length;
                    _fragments.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }

            return totalFreed;
        }



        internal int UpdateObj(mOSObject obj, byte[] objNewBin)
        {
            // 1️⃣ Localiza o objeto raiz
            HeapObject? root = _objects.FirstOrDefault(o => o.VirtualHeapAddr == obj.VirtualAddr);
            if (root == null)
                return 0;

            // 2️⃣ Coleta todas as partes (root + continuations)
            List<HeapObject> parts = new List<HeapObject>();
            HeapObject current = root;
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

                // cria HeapObject da continuação
                HeapObject newPart = new HeapObject
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
                HeapObject last = parts.Last();
                last.ContinuationAddr = continuationAddr;

                byte[] contPtr = BitConverter.GetBytes(continuationAddr);
                Write(last.VirtualHeapAddr + 4, ref contPtr);

                _objects.Add(newPart);


                HeapFragment? fg = _fragments.FirstOrDefault(f => f.VirtualHeapStart == last.ContinuationAddr);
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

    }
}