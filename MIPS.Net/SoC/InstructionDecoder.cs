using System;
using System.Runtime.CompilerServices;
using MIPS.Net.InstructionSet;

namespace MIPS.Net.SoC
{
    internal class InstructionDecoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void DecodeAndExec(int instruction, Registers rx)
        {
            // Atalho rápido para a instrução especial syscall literal
            if (instruction == 0x1F)
            {
                syscall.__call(ref rx);
                return;
            }

            int opcode = (instruction >> 26) & 0x3F;

            switch (opcode)
            {
                case 0x00: // Tipo-R (Usa o campo funct)
                    int funct = instruction & 0x3F;
                    switch (funct)
                    {
                        case 0x10: mfhi.__call(instruction, ref rx); break;
                        case 0x12: mflo.__call(instruction, ref rx); break;
                        case 0x1A: mul.__call(instruction, ref rx); break;
                        case 0x22: add.__call(instruction, ref rx); break; // se opcode 00/funct 22 for add
                        case 0x24: seq.__call(instruction, ref rx); break;
                        case 0x25: sne.__call(instruction, ref rx); break;
                        case 0x26: sgeu.__call(instruction, ref rx); break;
                        case 0x27: sgtu.__call(instruction, ref rx); break;
                        case 0x29: div.__call(instruction, ref rx); break; // duplo mapeamento corrigido pelo switch
                        case 0x2A: slt.__call(instruction, ref rx); break;
                        case 0x2B: sltu.__call(instruction, ref rx); break;
                        default: break; // Trata instruções não implementadas sem travar
                    }
                    break;

                // Tipo-I e Tipo-J (Usa apenas o Opcode)
                case 0x01: break; // bgez
                case 0x02: break; // bgtz
                case 0x03: jal.__call(instruction, ref rx); break;
                case 0x04: break; // bltz
                case 0x05: break; // bne
                case 0x06: break; // blez
                case 0x07: break; // bgtz
                case 0x08: addi.__call(instruction, ref rx); break;
                case 0x09: addiu.__call(instruction, ref rx); break;
                case 0x0A: move.__call(instruction, ref rx); break;
                case 0x0C: andi.__call(instruction, ref rx); break;
                case 0x0D: ori.__call(instruction, ref rx); break;
                case 0x0E: xori.__call(instruction, ref rx); break;
                case 0x0F: lui.__call(instruction, ref rx); break;
                case 0x13: lar.__call(instruction, ref rx); break;
                case 0x14: beq.__call(instruction, ref rx); break;
                case 0x20: lb.__call(instruction, rx); break;
                case 0x21: lh.__call(instruction, ref rx); break;
                case 0x22: add.__call(instruction, ref rx); break; // Verifique se seu add original usava opcode 0x22 ou funct
                case 0x23: lw.__call(instruction, rx); break;
                case 0x24: lbu.__call(instruction, ref rx); break;
                case 0x26: la.__call(instruction, ref rx); break;
                case 0x28: sb.__call(instruction, rx); break;
                case 0x2B: sw.__call(instruction, rx); break;
                case 0x30: jr.__call(instruction, ref rx); break;
                case 0x3E: li.__call(instruction, ref rx); break;
                case 0x3F: jalf.__call(instruction, ref rx); break;

                // Opcodes vazios mapeados direto para eliminação de branch
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x25:
                case 0x29:
                case 0x2A:
                case 0x2C:
                case 0x2D:
                case 0x2E:
                case 0x2F:
                case 0x31:
                case 0x32:
                case 0x33:
                    // ... agrupar todos os vazios aqui reduz o tamanho da tabela do JIT
                    break;

                default:
                    // Deixe comentado ou remova a exceção em benchmarks agressivos para evitar barreiras de otimização do JIT
                    // throw new Exception($"Unsupported opcode '0x{opcode:X2}'");
                    break;
            }
        }
    }
}