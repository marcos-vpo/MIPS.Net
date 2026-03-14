using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;
using MIPS.Net.SoC.__program;

namespace MIPS.Net.InstructionSet
{
    public class la
    {
        // Carrega o endereço de uma label ou variável em um registrador.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            // Calcula o endereço da label/variável
            int address = immediate; // Valor imediato (offset)

            DataEntry? dataEntryAddr = MIPS_CPU.CurrentProgram.GetDataEntryByRelativeAddr(address);
            if (dataEntryAddr == null) return;

            // Armazena o endereço no registrador de destino (rt)
            registers[rt] = dataEntryAddr.Address + 6;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "la"
            if (parts[0] != "la")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'la'.");
            }

            // 3. Extrair o registrador de destino e o endereço do rótulo
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            int labelAddress = int.Parse(parts[2]); // Extrai o endereço do rótulo como inteiro

            rd = rd.Substring(0, rd.Length - 1);

            // 4. Converter o nome do registrador para seu código numérico
            int rdValue = registers.CodeByName(rd);


            // 6. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 0x26 << 26; // Opcode para la (0x26) 
            instructionValue |= 0 << 21; // rs (0)
            instructionValue |= rdValue << 16;
            instructionValue |= labelAddress & 0xFFFF;  // Valor imediato (16 bits)

            // 7. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
