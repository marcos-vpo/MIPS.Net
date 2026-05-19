using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    // direct memory access module
    public class DMA
    {
        private static DMA Instance { get; set; }
        private readonly Memory _memory;

        internal DMA(Memory mem)
        {
            _memory = mem;
            Instance = this;
        }

        public static void RequestData(int address, ref byte[] buffer, bool noMMU = false)
        {
            bool mmu = false;
            if (MIPS_CPU.CurrentProgram != null)
                mmu = MIPS_CPU.CurrentProgram.IsMMUEnabled;

            if (mmu == false)
            {
                if (address > Instance._memory.Size)
                {
                    buffer = new byte[0];
                    return;
                }
                if (buffer.Length > Instance._memory.Size)
                {
                    buffer = new byte[0];
                    return;
                }
            }

            if (mmu && noMMU == false)
            {
                int virtualAddress = address;
                if (MMU.HasPageSpace(virtualAddress, buffer.Length))
                {

                    int f_address = MMU.TranslateAddr(virtualAddress);
                    Instance._memory.ReadBuffer(f_address, ref buffer);
                }
                else
                {
                    int totalRemaining = buffer.Length;
                    int readed = 0;
                    while (totalRemaining > 0)
                    {
                        int pageLeftSpace = MMU.RemainingPageSpace(virtualAddress);
                        if(pageLeftSpace > totalRemaining)
                        {
                            pageLeftSpace = totalRemaining;
                        }
                        byte[] spaceBuffer = new byte[pageLeftSpace];

                        int f_address = MMU.TranslateAddr(virtualAddress);
                        Instance._memory.ReadBuffer(f_address, ref spaceBuffer);

                        Array.Copy(spaceBuffer, 0, buffer, readed, spaceBuffer.Length);

                        readed += pageLeftSpace;
                        totalRemaining -= pageLeftSpace;
                        virtualAddress += pageLeftSpace;



                    }
                }
     
            }

            else
            {
                if (IOBUS.DEVICE_MAPPED(address) && buffer[0] == 0x01)
                {
                    AutoResetEvent ar = new AutoResetEvent(false);
                    IOBUS.SEND(
                       operation: 'R', // request read
                       address: address,
                       data: [0],
                       callBack: (res) =>
                       {
                           ar.Set();
                           return 0;
                       });
                    ar.WaitOne();
                }
                else
                    Instance._memory.ReadBuffer(address, ref buffer);
            }
        }

        public static void StoreData(int address, ref byte[] buffer, bool noMMU = false)
        {
            bool mmu = false;
            if (MIPS_CPU.CurrentProgram != null)
                mmu = MIPS_CPU.CurrentProgram.IsMMUEnabled;

            if (mmu == false)
            {
                if (address > Instance._memory.Size)
                {
                    buffer = new byte[0];
                    return;
                }
                if (buffer.Length > Instance._memory.Size)
                {
                    buffer = new byte[0];
                    return;
                }
            }

            if (mmu && noMMU == false)
            {
                int virtualAddress = address;
                if (!MMU.HasPageSpace(virtualAddress, buffer.Length))
                    throw new Exception($"No space left in target memory page for {buffer.Length} bytes");
                address = MMU.TranslateAddr(virtualAddress);
            }

            if (IOBUS.DEVICE_MAPPED(address) && buffer[0] == 0x01)
            {
                AutoResetEvent ar = new AutoResetEvent(false);
                IOBUS.SEND(
                   operation: 'W', // request write
                   address: address,
                   data: [0],
                   callBack: (res) =>
                   {
                       ar.Set();
                       return 0;
                   });
                ar.WaitOne();
            }
            else
                Instance._memory.StoreBuffer(address, ref buffer);
        }
    }
}
