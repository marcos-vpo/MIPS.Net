using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mOS.process_heap.ph_obj;

namespace mOS.process
{
    // Representa uma entrada da seção .data
    public class DataEntryInfo
    {
        public int Index { get; set; }
        public DataType Type { get; set; }
        public byte[] Data { get; set; }

        // Endereços calculados
        public int VirtualAddress { get; set; }
        public int PhysicalAddress { get; set; }
    }

    // Representa um rótulo (.text)
    public class RotuleInfo
    {
        public int Index { get; set; }
        public int RelativeAddress { get; set; }
        public int Length { get; set; }

        // Endereços calculados
        public int VirtualAddress { get; set; }
        public int PhysicalAddress { get; set; }
    }

    // Tipos suportados no .data
    public enum DataType : short
    {
        Int32 = 1,
        Int16 = 2,
        Byte = 3,
        String = 4,
        Float = 5,
        Double = 6
    }

    internal class ProgramParser
    {
        private readonly byte[] _programBytes;
        private readonly IReadOnlyCollection<ProcessHeapPage> pages;
        private readonly int heapVirtualAddr;
        private readonly int physicalAddr;
        private int _offset = 0;

        public byte Version { get; private set; }
        public int ProgramLength { get; private set; }

        public List<DataEntryInfo> DataEntries { get; private set; } = new List<DataEntryInfo>();
        public List<RotuleInfo> Rotules { get; private set; } = new List<RotuleInfo>();

        public ProgramParser(byte[] programBytes, IReadOnlyCollection<ProcessHeapPage> pages, int heapVirtualAddr, int physicalAddr)
        {
            _programBytes = programBytes ?? throw new ArgumentNullException(nameof(programBytes));
            this.pages = pages ?? throw new ArgumentNullException(nameof(pages));
            this.heapVirtualAddr = heapVirtualAddr;
            this.physicalAddr = physicalAddr;

            ParseHeader();
            ParseDataSection();
            ParseRotulesSection();
        }

        private void ParseHeader()
        {
            if (_programBytes.Length < 6)
                throw new Exception("Programa inválido ou corrompido.");

            if (_programBytes[_offset] != 0xD7)
                throw new Exception("Programa inválido: byte de início não encontrado.");

            _offset += 1;
            Version = _programBytes[_offset++];
            ProgramLength = BitConverter.ToInt32(_programBytes, _offset);
            _offset += 4;
        }

        private void ParseDataSection()
        {
            int dataSectionSize = BitConverter.ToInt32(_programBytes, _offset);
            _offset += 4;

            int readed = 0;
            int dataIndex = 0;

            while (readed < dataSectionSize)
            {
                DataType type = (DataType)BitConverter.ToInt16(_programBytes, _offset);
                _offset += 2;

                int entryLen = BitConverter.ToInt32(_programBytes, _offset);
                _offset += 4;

                byte[] entryData = new byte[entryLen];
                Array.Copy(_programBytes, _offset, entryData, 0, entryLen);
                _offset += entryLen;

                // Calcula endereços físico e virtual
                int relAddr = _offset - entryLen - 6; // posição relativa dentro do programa
                int virtualAddr = heapVirtualAddr + relAddr;
                int physicalAddr = ResolvePhysicalAddress(relAddr);

                DataEntries.Add(new DataEntryInfo
                {
                    Index = dataIndex++,
                    Type = type,
                    Data = entryData,
                    VirtualAddress = virtualAddr,
                    PhysicalAddress = physicalAddr
                });

                readed += 2 + 4 + entryLen;
            }
        }

        private void ParseRotulesSection()
        {
            int rotulesSectionSize = BitConverter.ToInt32(_programBytes, _offset);
            _offset += 4;

            int readed = 0;
            while (readed < rotulesSectionSize)
            {
                int idx = BitConverter.ToInt32(_programBytes, _offset);
                _offset += 4;

                int relAddr = BitConverter.ToInt32(_programBytes, _offset);
                _offset += 4;

                int len = BitConverter.ToInt32(_programBytes, _offset);
                _offset += 4;

                int virtualAddr = heapVirtualAddr + relAddr;
                int physicalAddr = ResolvePhysicalAddress(relAddr);

                Rotules.Add(new RotuleInfo
                {
                    Index = idx,
                    RelativeAddress = relAddr,
                    Length = len,
                    VirtualAddress = virtualAddr,
                    PhysicalAddress = physicalAddr
                });

                readed += 12;
            }
        }

        private int ResolvePhysicalAddress(int relAddr)
        {
            // Identifica a página correspondente
            ProcessHeapPage page = pages.FirstOrDefault(p =>
                relAddr >= (p.Order * HeapPageSize()) && relAddr < ((p.Order + 1) * HeapPageSize())
            );

            if (page == null)
                throw new Exception($"Não foi possível resolver endereço físico para relAddr={relAddr}");

            int offsetInPage = relAddr - (page.Order * HeapPageSize());
            return page.PageIndex * HeapPageSize() + offsetInPage;
        }

        private int HeapPageSize()
        {
            // tamanho fixo da página; adapte conforme seu heap
            return 4096;
        }

        public int? GetRotuleAddress(int rotuleIndex)
        {
            var r = Rotules.FirstOrDefault(rtl => rtl.Index == rotuleIndex);
            return r?.VirtualAddress;
        }

        public DataEntryInfo? GetDataEntry(int index)
        {
            return DataEntries.FirstOrDefault(d => d.Index == index);
        }
    }
}
