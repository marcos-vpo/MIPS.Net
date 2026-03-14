using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class lw
    {   
        
        // Carrega uma palavra da memória em um registrador.
        public static void __call(int instruction, Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int offset = instruction & 0xFFFF;

            int address = registers[rs] + offset;

            byte[] val = new byte[4];
            MemoryBUS.SEND('R', address, val,(KeyValuePair<bool, byte[]> ret) =>
            {
                registers[rt] = BitConverter.ToInt32(ret.Value);
                return 0;
            });
  
        }


        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "lw"
            if (parts[0] != "lw")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'lw'.");
            }

            // 3. Extrair os registradores e o valor imediato
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            string offsetPart = parts[2]; // Extrai a parte do offset com parênteses

            // 4. Extrair o offset e o registrador de base
            int offset = int.Parse(offsetPart.Substring(0, offsetPart.IndexOf('('))); // Remove parênteses e converte para inteiro
            string baseRegisterName = offsetPart.Substring(offsetPart.IndexOf('(') + 1, offsetPart.IndexOf(')') - offsetPart.IndexOf('(') - 1); // Extrai o nome do registrador de base

            rd = rd.Substring(0, rd.Length - 1);

            // 5. Converter os nomes dos registradores para seus valores numéricos
            int rdValue = registers.CodeByName(rd);
            int rsValue = registers.CodeByName(baseRegisterName);

            // 6. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 0x23 << 26; // Opcode para lw (0x23)
            instructionValue |= rsValue << 21;
            instructionValue |= rdValue << 16;
            instructionValue |= offset & 0xFFFF;  // Valor imediato (16 bits)

            // 7. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
