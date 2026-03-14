using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class jal
    {
        // Salta para o endereço especificado e salva o endereço da próxima instrução em $ra.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai o endereço de destino (target) da instrução
            int target = instruction & 0x03FFFFFF; // 26 bits menos significativos

            bool avoidSaveState = registers[$"k1"] == 1;

           if (avoidSaveState == false)
                registers[Registers.RA] = registers[Registers.PC];              // Salva o endereço da próxima instrução em $ra (PC + 4)
          //  else
            {

            }
                var rotule = MIPS_CPU.CurrentProgram.GetRotuleByRelativeAddr(target);
            if (rotule == null)
                MIPS_CPU.Instance.RequestHalt();
            else
            // Atualiza o PC com o endereço de destino
            {
                registers[Registers.PC] = rotule.AbsoluteAddr - 4;

               // if (avoidSaveState == false)
                    MIPS_CPU.Instance.DBG?.ProgramSwitching(MIPS_CPU.CurrentProgram, registers[Registers.PC] + 8);
            }
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "jal"
            if (parts[0] != "jal")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'jal'.");
            }

            // 3. Extrair o endereço de destino (target)
            int target = int.Parse(parts[1]); // Assumindo que o endereço é um inteiro

            // 4. Construir o valor da instrução MIPS (formato J-Type)
            int instructionValue = 0;
            instructionValue |= 0x03 << 26; // Opcode para jal (0x03)
            instructionValue |= target & 0x03FFFFFF; // Endereço de destino (26 bits)

            // 5. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
