using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.InstructionSet;
using static System.Net.Mime.MediaTypeNames;

namespace MIPS.Net.SoC
{
    internal class InstructionDecoder
    {
        public static void DecodeAndExec(int instruction, Registers rx)
        {
            // Extrai os 6 bits mais significativos (opcode)
            int opcode = (instruction >> 26) & 0x3F;
            int funct = instruction & 0x3F; // Extrai os 6 bits menos significativos (funct)


            // Define os opcodes do MIPS
            if (opcode == 0x08) addi.__call(instruction, ref rx); // addi
            else if (opcode == 0x3F) jalf.__call(instruction, ref rx);
            else if (opcode == 0x00 && funct == 0x29) div.__call(instruction, ref rx); // div
            else if (opcode == 0x00 && funct == 0x10) mfhi.__call(instruction, ref rx); // mfhi
            else if (opcode == 0x00 && funct == 0x12) mflo.__call(instruction, ref rx); // mflo
            else if (opcode == 0x0A) move.__call(instruction, ref rx); // tge
            else if (opcode == 0x00 && funct == 0x1A) mul.__call(instruction, ref rx); // mul
            else if (opcode == 0x09) addiu.__call(instruction, ref rx); // addiu
            else if (opcode == 0x0C) andi.__call(instruction, ref rx); // andi
            else if (opcode == 0x0D) ori.__call(instruction, ref rx); // ori
            else if (opcode == 0x0E) xori.__call(instruction, ref rx); // xori
            else if (opcode == 0x0F) lui.__call(instruction, ref rx); // lui
            else if (opcode == 0x20) lb.__call(instruction, rx); // lb
            else if (opcode == 0x28) sb.__call(instruction, rx); // sb
            else if (opcode == 0x21) lh.__call(instruction, ref rx); // lh
            else if (opcode == 0x23) lw.__call(instruction, rx); // lw
            else if (opcode == 0x24) lbu.__call(instruction, ref rx); // lbu
            else if (instruction == 0x1F) syscall.__call(ref rx);
            else if (opcode == 0x22) add.__call(instruction, ref rx);
            else if (opcode == 0x26) la.__call(instruction, ref rx); // la
            else if (opcode == 0x13) lar.__call(instruction, ref rx); // la
            else if (opcode == 0x2B) sw.__call(instruction, rx); // sw
            else if (opcode == 0x3E) li.__call(instruction, ref rx); // li (dedicated)
            else if (opcode == 0x03) jal.__call(instruction, ref rx); // jal
            else if (opcode == 0x30) jr.__call(instruction, ref rx); // jr
            else if (opcode == 0x25) ; // lhu
            else if (opcode == 0x29) ; // sh
            else if (opcode == 0x14) beq.__call(instruction, ref rx);
            else if (opcode == 0x15) ; // bne
            else if (opcode == 0x04) ; // bltz
            else if (opcode == 0x05) ; // bne
            else if (opcode == 0x01) ; // bgez
            else if (opcode == 0x02) ; // bgtz
            else if (opcode == 0x06) ; // blez
            else if (opcode == 0x07) ; // bgtz
            else if (opcode == 0x16) ; // blez
            else if (opcode == 0x17) ; // bgtz
            else if (opcode == 0x18) ; // addiu
            else if (opcode == 0x19) ; // sltiu

            else if (opcode == 0x1B) ; // ori
            else if (opcode == 0x1C) ; // xori
            else if (opcode == 0x1D) ; // lui
            else if (opcode == 0x2A) ; // sll
            else if (opcode == 0x2B) ; // srl
            else if (opcode == 0x2C) ; // sra
            else if (opcode == 0x2D) ; // sllv
            else if (opcode == 0x2E) ; // srlv
            else if (opcode == 0x2F) ; // srav
            else if (opcode == 0x31) ; // jalr
            else if (opcode == 0x32) ; // movz
            else if (opcode == 0x33) ; // movn
            else if (opcode == 0x34) ; // syscall
            else if (opcode == 0x35) ; // break
            else if (opcode == 0x36) ; // sync



            else if (opcode == 0x38) ; // mthi
            else if (opcode == 0x3A) ; // mtlo

            else if (opcode == 0x3B) ; // dsllv
            else if (opcode == 0x3C) ; // dsrlv
            else if (opcode == 0x3D) ; // dsrav

            else if (opcode == 0x40) ; // tge
            else if (opcode == 0x41) ; // tlt
            else if (opcode == 0x42) ; // tgeu
            else if (opcode == 0x43) ; // tltu
            else if (opcode == 0x46) ; // teq
            else if (opcode == 0x47) ; // tge
            else if (opcode == 0x48) ; // tlt
            else if (opcode == 0x49) ; // tgeu
            else if (opcode == 0x4A) ; // tltu
            else if (opcode == 0x4B) ; // bltzal
            else if (opcode == 0x4C) ; // bgezal
            else if (opcode == 0x4D) ; // bgezal
            else if (opcode == 0x4E) ; // bgezal
            else if (opcode == 0x4F) ; // bgezal
            else if (opcode == 0x50) ; // tge
            else if (opcode == 0x51) ; // tlt
            else if (opcode == 0x52) ; // tgeu
            else if (opcode == 0x53) ; // tltu
            else if (opcode == 0x54) ; // tge
            else if (opcode == 0x55) ; // tlt
            else if (opcode == 0x56) ; // tgeu
            else if (opcode == 0x57) ; // tltu
            else if (opcode == 0x58) ; // teq
            else if (opcode == 0x59) ; // tge
            else if (opcode == 0x5A) ; // tlt
            else if (opcode == 0x5B) ; // tgeu
            else if (opcode == 0x5C) ; // tltu
            else if (opcode == 0x5D) ; // bgezal
            else if (opcode == 0x5E) ; // bgezal
            else if (opcode == 0x5F) ; // bgezal
            else if (opcode == 0x60) ; // tge
            else if (opcode == 0x61) ; // tlt
            else if (opcode == 0x62) ; // tgeu
            else if (opcode == 0x63) ; // tltu
            else if (opcode == 0x64) ; // tge
            else if (opcode == 0x65) ; // tlt
            else if (opcode == 0x66) ; // tgeu
            else if (opcode == 0x67) ; // tltu
            else if (opcode == 0x68) ; // tge
            else if (opcode == 0x69) ; // tlt
            else if (opcode == 0x6A) ; // tgeu
            else if (opcode == 0x6B) ; // tltu
            else if (opcode == 0x6C) ; // bgezal
            else if (opcode == 0x6D) ; // bgezal
            else if (opcode == 0x6E) ; // bgezal
            else if (opcode == 0x6F) ; // bgezal
            else if (opcode == 0x70) ; // tge
            else if (opcode == 0x71) ; // tlt
            else if (opcode == 0x72) ; // tgeu
            else if (opcode == 0x73) ; // tltu
            else if (opcode == 0x74) ; // tge
            else if (opcode == 0x75) ; // tlt
            else if (opcode == 0x76) ; // tgeu
            else if (opcode == 0x77) ; // tltu
            else if (opcode == 0x78) ; // tge
            else if (opcode == 0x79) ; // tlt
            else if (opcode == 0x7A) ; // tgeu
            else if (opcode == 0x7B) ; // tltu
            else if (opcode == 0x7C) ; // bgezal
            else if (opcode == 0x7D) ; // bgezal
            else if (opcode == 0x81) ; // addu
            else if (opcode == 0x82) ; // sub
            else if (opcode == 0x83) ; // subu
            else if (opcode == 0x84) ; // and
            else if (opcode == 0x85) ; // or
            else if (opcode == 0x86) ; // xor
            else if (opcode == 0x87) ; // nor
            else if (opcode == 0x00 && funct == 0x2A) slt.__call(instruction, ref rx);
            else if (opcode == 0x00 && funct == 0x2B) sltu.__call(instruction, ref rx); // Decodifica e executa sltu
            else if (opcode == 0x00 && funct == 0x29) sgt.__call(instruction, ref rx); // Decodifica e executa sgt
            else if (opcode == 0x00 && funct == 0x27) sgtu.__call(instruction, ref rx);
            else if (opcode == 0x00 && funct == 0x24) seq.__call(instruction, ref rx); // Decodifica e executa seq
            else if (opcode == 0x00 && funct == 0x25) sne.__call(instruction, ref rx); // Decodifica e executa sne
            else if (opcode == 0x00 && funct == 0x22) sge.__call(instruction, ref rx); // Decodifica e executa sge
            else if (opcode == 0x00 && funct == 0x26) sgeu.__call(instruction, ref rx); // Decodifica e executa sgeu

            else if (opcode == 0x8A) ; // tge
            else if (opcode == 0x8B) ; // tlt
            else if (opcode == 0x8C) ; // teq
            else if (opcode == 0x8D) ; // tne
            else if (opcode == 0x8E) ; // tgeu
            else if (opcode == 0x8F) ; // tltu
            else if (opcode == 0x90) ; // dadd
            else if (opcode == 0x91) ; // daddu
            else if (opcode == 0x92) ; // dsub
            else if (opcode == 0x93) ; // dsubu
            else if (opcode == 0x94) ; // tge
            else if (opcode == 0x95) ; // tlt
            else if (opcode == 0x96) ; // teq
            else if (opcode == 0x97) ; // tne
            else if (opcode == 0x98) ; // tgeu
            else if (opcode == 0x99) ; // tltu
            else if (opcode == 0xA0) ; // mult
            else if (opcode == 0xA1) ; // multu

            else if (opcode == 0xA3) ; // divu
            else if (opcode == 0xA4) ; // dmult
            else if (opcode == 0xA5) ; // dmultu
            else if (opcode == 0xA6) ; // ddiv
            else if (opcode == 0xA7) ; // ddivu
            else if (opcode == 0xAC) ; // mthi
            else if (opcode == 0xAD) ; // mtlo
            else if (opcode == 0xAE) ; // mfhi
            else if (opcode == 0xAF) ; // mflo
            else if (opcode == 0xB0) ; // dadd
            else if (opcode == 0xB1) ; // daddu
            else if (opcode == 0xB2) ; // dsub
            else if (opcode == 0xB3) ; // dsubu
            else if (opcode == 0xB4) ; // dmult
            else if (opcode == 0xB5) ; // dmultu
            else if (opcode == 0xB6) ; // ddiv
            else if (opcode == 0xB7) ; // ddivu
            else if (opcode == 0xBC) ; // dadd
            else if (opcode == 0xBD) ; // daddu
            else if (opcode == 0xBE) ; // dsub
            else if (opcode == 0xBF) ; // dsubu
            else if (opcode == 0xC0) ; // tge
            else if (opcode == 0xC1) ; // tlt
            else if (opcode == 0xC2) ; // teq
            else if (opcode == 0xC3) ; // tne
            else if (opcode == 0xC4) ; // tgeu
            else if (opcode == 0xC5) ; // tltu
            else if (opcode == 0xC6) ; // tge
            else if (opcode == 0xC7) ; // tlt
            else if (opcode == 0xC8) ; // teq
            else if (opcode == 0xC9) ; // tne
            else if (opcode == 0xCA) ; // tgeu
            else if (opcode == 0xCB) ; // tltu
            else if (opcode == 0xCC) ; // tge
            else if (opcode == 0xCD) ; // tlt
            else if (opcode == 0xCE) ; // teq
            else if (opcode == 0xCF) ; // tne
            else if (opcode == 0xD0) ; // tgeu
            else if (opcode == 0xD1) ; // tltu
            else if (opcode == 0xD2) ; // tge
            else if (opcode == 0xD3) ; // tlt
            else if (opcode == 0xD4) ; // teq
            else if (opcode == 0xD5) ; // tne
            else if (opcode == 0xD6) ; // tgeu
            else if (opcode == 0xD7) ; // tltu
            else if (opcode == 0xD8) ; // tge
            else if (opcode == 0xD9) ; // tlt
            else if (opcode == 0xDA) ; // teq
            else if (opcode == 0xDB) ; // tne
            else if (opcode == 0xDC) ; // tgeu
            else if (opcode == 0xDD) ; // tltu
            else if (opcode == 0xDE) ; // tge
            else if (opcode == 0xDF) ; // tlt
            else if (opcode == 0xE0) ; // teq
            else if (opcode == 0xE1) ; // tne
            else if (opcode == 0xE2) ; // tgeu
            else if (opcode == 0xE3) ; // tltu
            else if (opcode == 0xE4) ; // tge
            else if (opcode == 0xE5) ; // tlt
            else if (opcode == 0xE6) ; // teq
            else if (opcode == 0xE7) ; // tne
            else if (opcode == 0xE8) ; // tgeu
            else if (opcode == 0xE9) ; // tltu
            else if (opcode == 0xEA) ; // tge
            else if (opcode == 0xEB) ; // tlt
            else if (opcode == 0xEC) ; // teq
            else if (opcode == 0xED) ; // tne
            else if (opcode == 0xEE) ; // tgeu
            else if (opcode == 0xEF) ; // tltu
            else if (opcode == 0xF0) ; // tge
            else if (opcode == 0xF1) ; // tlt
            else if (opcode == 0xF2) ; // teq
            else if (opcode == 0xF3) ; // tne
            else if (opcode == 0xF4) ; // tgeu
            else if (opcode == 0xF5) ; // tltu
            else if (opcode == 0xF6) ; // tge
            else if (opcode == 0xF7) ; // tlt
            else if (opcode == 0xF8) ; // teq
            else if (opcode == 0xF9) ; // tne
            else if (opcode == 0xFA) ; // tgeu
            else if (opcode == 0xFB) ; // tltu
            else if (opcode == 0xFC) ; // tge
            else if (opcode == 0xFD) ; // tlt
            else if (opcode == 0xFE) ; // teq
            else if (opcode == 0xFF) ; // tne
            else throw new Exception($"Unsupported opcode '0x{opcode:X2}'");
        }
    }
}
