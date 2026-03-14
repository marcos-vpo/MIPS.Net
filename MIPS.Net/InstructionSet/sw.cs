using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class sw
    {
        // Armazena um inteiro de 32 bits em um endereço de memória.
        public static void __call(int instruction, Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int pos = instruction & 0xFFFF;
            
            // Calcula o endereço da memória
            int address = registers[rt] + pos;
            int val = registers[rs]; // valor no registrador

            byte[] buffer = BitConverter.GetBytes(val);

            MemoryBUS.SEND('W', address, buffer);
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "sw"
            if (parts[0] != "sw")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'sw'.");
            }

            // 3. Extrair os registradores e o valor imediato
            string rs = parts[1].Substring(1); // Remove o '$' do nome do registrador
            string offsetPart = parts[2]; // Extrai a parte do offset com parênteses

            // 4. Extrair o offset e o registrador de base
            int offset = int.Parse(offsetPart.Substring(0, offsetPart.IndexOf('('))); // Remove parênteses e converte para inteiro
            string baseRegisterName = offsetPart.Substring(offsetPart.IndexOf('(') + 1, offsetPart.IndexOf(')') - offsetPart.IndexOf('(') - 1); // Extrai o nome do registrador de base

            rs = rs.Substring(0, rs.Length - 1);

            // 5. Converter os nomes dos registradores para seus valores numéricos
            int rsValue = registers.CodeByName(rs);
            int rtValue = registers.CodeByName(baseRegisterName);

            // 6. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 0x2B << 26; // Opcode para sw (0x2B)
            instructionValue |= rsValue << 21;
            instructionValue |= rtValue << 16;
            instructionValue |= offset & 0xFFFF;  // Valor imediato (16 bits)

            // 7. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
