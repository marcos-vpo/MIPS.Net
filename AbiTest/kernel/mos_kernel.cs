using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using MIPS.Net.abi;


namespace mOS.kernel
{
    public class mos_kernel : ABIManaged
    {
        [Extern]
        public int start()
        {
            bootstrap();
            return 0; // nunca deveria retornar, mas mantém ABI feliz
        }

        private void bootstrap()
        {
            init_memory();
            init_interrupts();
            init_devices();
        }

        private void init_memory()
        {
            byte[] mb = new byte[1];
            int addr = 6010000;
            int m_len = 0;
            while (true)
            {
                read(addr, ref mb);
                m_len += 1;
                addr += 1;

                if (mb[0] == 0xD7) break;
            }


            // ✔️ heap
            // ✔️ page tables (se houver)
            // ✔️ mapear devices (ex: vídeo em 4096)
        }

        private static void init_interrupts()
        {
            // ✔️ inicializar controlador
            // ✔️ limpar tabela
            // ✔️ definir handler default
        }

        private static void init_devices()
        {
            // ✔️ vídeo
            // ✔️ storage
            // ✔️ usb (quando entrar)
        }

      
    }
}

