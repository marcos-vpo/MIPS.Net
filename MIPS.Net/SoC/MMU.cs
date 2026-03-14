using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    public class TLBEntry
    {
        public TLBEntry(int page)
        {
            PhysicalPage = page;
        }

        public int PhysicalPage { get; set; }
        public int Offset { get; set; }
    }

    public class MMU
    {
        public bool Enabled => entries.Count > 0;
        public static int PageSize { get; set; }
        public static int PageAreaStart { get; set; }
        public static int PageAreaEnd { get; set; }

        private static MMU _instance;
        public MMU()
        {
            _instance = this;
        }


        public void Initialize(int page_area_start, int page_size)
        {
            PageAreaStart = page_area_start;
            PageSize = page_size;
        }

        private List<TLBEntry> entries = new List<TLBEntry>();
        private void SetWorkingSet(List<TLBEntry> entries)
        {
            this.entries = entries;
        }


        internal static void SetTLB(List<TLBEntry> tlb_entries)
        {
            _instance.SetWorkingSet(tlb_entries);
        }

        public int VirtualToPhysical(int virtualAddress)
        {
            // Sem MMU ativa, endereço lógico = endereço físico
            if (!Enabled)
                return virtualAddress;

            // Qual página virtual esse endereço pertence
            int virtualPageIndex = virtualAddress / PageSize;

            // Posição dentro da página virtual
            int offsetInsidePage = virtualAddress % PageSize;

            // Verifica se essa página virtual está mapeada
            if (virtualPageIndex < 0 || virtualPageIndex >= entries.Count)
                throw new Exception("Page fault: página virtual não mapeada");

            // Página física correspondente
            int physicalPageIndex = entries[virtualPageIndex].PhysicalPage;

            // Endereço físico final
            int physicalAddress =
                PageAreaStart +
                (physicalPageIndex * PageSize) +
                offsetInsidePage;

            return physicalAddress;
        }

        internal static bool IsEnabled()
        {
            return _instance.Enabled;
        }

        internal static int TranslateAddr(int address)
        {
            return _instance.VirtualToPhysical(address);
        }

        public static bool HasPageSpace(int virtualAddress, int sizeInBytes)
        {
            if (PageSize <= 0)
                throw new InvalidOperationException("MMU PageSize não inicializado");

            // Offset do endereço dentro da página
            int offsetInsidePage = virtualAddress % PageSize;

            // Quantos bytes ainda restam nessa página a partir do endereço
            int remainingBytesInPage = PageSize - offsetInsidePage;

            return sizeInBytes <= remainingBytesInPage;
        }

        public static int RemainingPageSpace(int virtualAddress)
        {
            if (PageSize <= 0)
                throw new InvalidOperationException("MMU PageSize não inicializado");

            // Offset do endereço dentro da página
            int offsetInsidePage = virtualAddress % PageSize;

            // Quantos bytes ainda restam nessa página a partir do endereço
            int remainingBytesInPage = PageSize - offsetInsidePage;

            return remainingBytesInPage;
        }

    }
}
