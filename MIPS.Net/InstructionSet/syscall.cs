using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class syscall
    {
        // Executa a syscall, considerando o código da syscall e os argumentos.
        public static void __call(ref Registers registers)
        {
            int syscallCode = registers["$v0"];
            InterruptionController.Instance.CallInterruption(syscallCode);
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 2. Verificar se a instrução é do tipo "syscall"
            if (instruction != "syscall")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'syscall'.");
            }

            // 3. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0x1F; // Opcode para syscall (0x1F) 

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
