using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.Debugger
{
    public  class RotuleInstructionVM
    {
        private static readonly string[] RegisterNames = {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra"
        };

        public RotuleInstructionVM(int instruction, int address, string srcFile = null, RotuleInstruction inst = null)
        {
            if (inst != null)
            {
                SrcCode = inst.Definition;
                FileInfo fi = new FileInfo(srcFile);
                SrcFile = $"{fi.Name}:ln {inst.Ln}";
            }

            InstructionCode = instruction;
            Address = address;
            AddressStr = $"0x{address.ToString("X4")}"; // Format address as hexadecimal

            DecodeInstruction(instruction);
        }

        public int Address { get; set; }
        public string AddressStr { get; set; }

        public int InstructionCode { get; set; }
        public string Instruction { get; set; }
        public int OpCode { get; set; }
        public string DR { get; set; }
        public string SR1 { get; set; }
        public string SR2 { get; set; }

        public string SrcCode { get; set; }
        public string SrcFile { get; set; }

        private string GetRegisterName(int registerNumber)
        {
            if (registerNumber >= 0 && registerNumber < RegisterNames.Length)
            {
                return RegisterNames[registerNumber];
            }
            return $"R{registerNumber}"; // Fallback for invalid register numbers
        }

        private void DecodeInstruction(int instruction)
        {
            // Extract opcode (assuming it's in the most significant bits)
            OpCode = (instruction >> 26) & 0x3F; // MIPS opcodes are 6 bits
            int funct = instruction & 0x3F;

            // Extract register information (assuming standard MIPS register layout)
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int rd = (instruction >> 11) & 0x1F;
            short immediate = (short)(instruction & 0xFFFF);

            // Assign registers based on the instruction type
            SR1 = GetRegisterName(rs);
            SR2 = GetRegisterName(rt);
            DR = GetRegisterName(rd); // Default, might be changed depending on the instruction.

            // Decode based on opcode and funct
            if (OpCode == 0x08) Instruction = "addi";
            else if (OpCode == 0x0A) Instruction = "tge";
            else if (OpCode == 0x00 && funct == 0x1A) Instruction = "mul";
            else if (OpCode == 0x09) Instruction = "addiu";
            else if (OpCode == 0x0C) Instruction = "andi";
            else if (OpCode == 0x0D) Instruction = "ori";
            else if (OpCode == 0x0E) Instruction = "xori";
            else if (OpCode == 0x0F) Instruction = "lui";
            else if (OpCode == 0x20) Instruction = "lb";
            else if (OpCode == 0x28) Instruction = "sb";
            else if (OpCode == 0x21) Instruction = "lh";
            else if (OpCode == 0x23) Instruction = "lw";
            else if (OpCode == 0x24) Instruction = "lbu";
            else if (instruction == 0x1F) Instruction = "syscall";
            else if (OpCode == 0x22) Instruction = "add";
            else if (OpCode == 0x26) Instruction = "la";
            else if (OpCode == 0x13) Instruction = "lar";
            else if (OpCode == 0x2B) Instruction = "sw";
            else if (OpCode == 0x3E) Instruction = "li";
            else if (OpCode == 0x03) Instruction = "jal";
            else if (OpCode == 0x30) Instruction = "jr";
            else if (OpCode == 0x25) Instruction = "lhu";
            else if (OpCode == 0x29) Instruction = "sh";
            else if (OpCode == 0x14) Instruction = "beq";
            else if (OpCode == 0x15) Instruction = "bne";
            else if (OpCode == 0x04) Instruction = "bltz";
            else if (OpCode == 0x05) Instruction = "bne";
            else if (OpCode == 0x01) Instruction = "bgez";
            else if (OpCode == 0x02) Instruction = "bgtz";
            else if (OpCode == 0x06) Instruction = "blez";
            else if (OpCode == 0x07) Instruction = "bgtz";
            else if (OpCode == 0x16) Instruction = "blez";
            else if (OpCode == 0x17) Instruction = "bgtz";
            else if (OpCode == 0x18) Instruction = "addiu";
            else if (OpCode == 0x19) Instruction = "sltiu";
            else if (OpCode == 0x1B) Instruction = "ori";
            else if (OpCode == 0x1C) Instruction = "xori";
            else if (OpCode == 0x1D) Instruction = "lui";
            else if (OpCode == 0x2A) Instruction = "sll";
            else if (OpCode == 0x2B) Instruction = "srl";
            else if (OpCode == 0x2C) Instruction = "sra";
            else if (OpCode == 0x2D) Instruction = "sllv";
            else if (OpCode == 0x2E) Instruction = "srlv";
            else if (OpCode == 0x2F) Instruction = "srav";
            else if (OpCode == 0x31) Instruction = "jalr";
            else if (OpCode == 0x32) Instruction = "movz";
            else if (OpCode == 0x33) Instruction = "movn";
            else if (OpCode == 0x34) Instruction = "syscall";
            else if (OpCode == 0x35) Instruction = "break";
            else if (OpCode == 0x36) Instruction = "sync";
            else if (OpCode == 0x37) Instruction = "mfhi";
            else if (OpCode == 0x38) Instruction = "mthi";
            else if (OpCode == 0x39) Instruction = "mflo";
            else if (OpCode == 0x3A) Instruction = "mtlo";
            else if (OpCode == 0x3B) Instruction = "dsllv";
            else if (OpCode == 0x3C) Instruction = "dsrlv";
            else if (OpCode == 0x3D) Instruction = "dsrav";
            else if (OpCode == 0x3F) Instruction = "teq";
            else if (OpCode == 0x40) Instruction = "tge";
            else if (OpCode == 0x41) Instruction = "tlt";
            else if (OpCode == 0x42) Instruction = "tgeu";
            else if (OpCode == 0x43) Instruction = "tltu";
            else if (OpCode == 0x46) Instruction = "teq";
            else if (OpCode == 0x47) Instruction = "tge";
            else if (OpCode == 0x48) Instruction = "tlt";
            else if (OpCode == 0x49) Instruction = "tgeu";
            else if (OpCode == 0x4A) Instruction = "tltu";
            else if (OpCode == 0x4B) Instruction = "bltzal";
            else if (OpCode == 0x4C) Instruction = "bgezal";
            else if (OpCode == 0x4D) Instruction = "bgezal";
            else if (OpCode == 0x4E) Instruction = "bgezal";
            else if (OpCode == 0x4F) Instruction = "bgezal";
            else if (OpCode == 0x50) Instruction = "tge";
            else if (OpCode == 0x51) Instruction = "tlt";
            else if (OpCode == 0x52) Instruction = "tgeu";
            else if (OpCode == 0x53) Instruction = "tltu";
            else if (OpCode == 0x54) Instruction = "tge";
            else if (OpCode == 0x55) Instruction = "tlt";
            else if (OpCode == 0x56) Instruction = "tgeu";
            else if (OpCode == 0x57) Instruction = "tltu";
            else if (OpCode == 0x58) Instruction = "teq";
            else if (OpCode == 0x59) Instruction = "tge";
            else if (OpCode == 0x5A) Instruction = "tlt";
            else if (OpCode == 0x5B) Instruction = "tgeu";
            else if (OpCode == 0x5C) Instruction = "tltu";
            else if (OpCode == 0x5D) Instruction = "bgezal";
            else if (OpCode == 0x5E) Instruction = "bgezal";
            else if (OpCode == 0x5F) Instruction = "bgezal";
            else if (OpCode == 0x60) Instruction = "tge";
            else if (OpCode == 0x61) Instruction = "tlt";
            else if (OpCode == 0x62) Instruction = "tgeu";
            else if (OpCode == 0x63) Instruction = "tltu";
            else if (OpCode == 0x64) Instruction = "tge";
            else if (OpCode == 0x65) Instruction = "tlt";
            else if (OpCode == 0x66) Instruction = "tgeu";
            else if (OpCode == 0x67) Instruction = "tltu";
            else if (OpCode == 0x68) Instruction = "tge";
            else if (OpCode == 0x69) Instruction = "tlt";
            else if (OpCode == 0x6A) Instruction = "tgeu";
            else if (OpCode == 0x6B) Instruction = "tltu";
            else if (OpCode == 0x6C) Instruction = "bgezal";
            else if (OpCode == 0x6D) Instruction = "bgezal";
            else if (OpCode == 0x6E) Instruction = "bgezal";
            else if (OpCode == 0x6F) Instruction = "bgezal";
            else if (OpCode == 0x70) Instruction = "tge";
            else if (OpCode == 0x71) Instruction = "tlt";
            else if (OpCode == 0x72) Instruction = "tgeu";
            else if (OpCode == 0x73) Instruction = "tltu";
            else if (OpCode == 0x74) Instruction = "tge";
            else if (OpCode == 0x75) Instruction = "tlt";
            else if (OpCode == 0x76) Instruction = "tgeu";
            else if (OpCode == 0x77) Instruction = "tltu";
            else if (OpCode == 0x78) Instruction = "tge";
            else if (OpCode == 0x79) Instruction = "tlt";
            else if (OpCode == 0x7A) Instruction = "tgeu";
            else if (OpCode == 0x7B) Instruction = "tltu";
            else if (OpCode == 0x7C) Instruction = "bgezal";
            else if (OpCode == 0x7D) Instruction = "bgezal";
            else if (OpCode == 0x7E) Instruction = "bgezal";
            else if (OpCode == 0x7F) Instruction = "bgezal";
            else if (OpCode == 0x81) Instruction = "addu";
            else if (OpCode == 0x82) Instruction = "sub";
            else if (OpCode == 0x83) Instruction = "subu";
            else if (OpCode == 0x84) Instruction = "and";
            else if (OpCode == 0x85) Instruction = "or";
            else if (OpCode == 0x86) Instruction = "xor";
            else if (OpCode == 0x87) Instruction = "nor";
            else if (OpCode == 0x00 && funct == 0x2A) Instruction = "slt";
            else if (OpCode == 0x00 && funct == 0x2B) Instruction = "sltu";
            else if (OpCode == 0x00 && funct == 0x29) Instruction = "sgt";
            else if (OpCode == 0x00 && funct == 0x27) Instruction = "sgtu";
            else if (OpCode == 0x00 && funct == 0x24) Instruction = "seq";
            else if (OpCode == 0x00 && funct == 0x25) Instruction = "sne";
            else if (OpCode == 0x00 && funct == 0x22) Instruction = "sge";
            else if (OpCode == 0x00 && funct == 0x26) Instruction = "sgeu";
            else if (OpCode == 0x8A) Instruction = "tge";
            else if (OpCode == 0x8B) Instruction = "tlt";
            else if (OpCode == 0x8C) Instruction = "teq";
            else if (OpCode == 0x8D) Instruction = "tne";
            else if (OpCode == 0x8E) Instruction = "tgeu";
            else if (OpCode == 0x8F) Instruction = "tltu";
            else if (OpCode == 0x90) Instruction = "dadd";
            else if (OpCode == 0x91) Instruction = "daddu";
            else if (OpCode == 0x92) Instruction = "dsub";
            else if (OpCode == 0x93) Instruction = "dsubu";
            else if (OpCode == 0x94) Instruction = "tge";
            else if (OpCode == 0x95) Instruction = "tlt";
            else if (OpCode == 0x96) Instruction = "teq";
            else if (OpCode == 0x97) Instruction = "tne";
            else if (OpCode == 0x98) Instruction = "tgeu";
            else if (OpCode == 0x99) Instruction = "tltu";
            else if (OpCode == 0xA0) Instruction = "mult";
            else if (OpCode == 0xA1) Instruction = "multu";
            else if (OpCode == 0xA2) Instruction = "div";
            else if (OpCode == 0xA3) Instruction = "divu";
            else if (OpCode == 0xA4) Instruction = "dmult";
            else if (OpCode == 0xA5) Instruction = "dmultu";
            else if (OpCode == 0xA6) Instruction = "ddiv";
            else if (OpCode == 0xA7) Instruction = "ddivu";
            else if (OpCode == 0xAC) Instruction = "mthi";
            else if (OpCode == 0xAD) Instruction = "mtlo";
            else if (OpCode == 0xAE) Instruction = "mfhi";
            else if (OpCode == 0xAF) Instruction = "mflo";
            else if (OpCode == 0xB0) Instruction = "dadd";
            else if (OpCode == 0xB1) Instruction = "daddu";
            else if (OpCode == 0xB2) Instruction = "dsub";
            else if (OpCode == 0xB3) Instruction = "dsubu";
            else if (OpCode == 0xB4) Instruction = "dmult";
            else if (OpCode == 0xB5) Instruction = "dmultu";
            else if (OpCode == 0xB6) Instruction = "ddiv";
            else if (OpCode == 0xB7) Instruction = "ddivu";
            else if (OpCode == 0xBC) Instruction = "dadd";
            else if (OpCode == 0xBD) Instruction = "daddu";
            else if (OpCode == 0xBE) Instruction = "dsub";
            else if (OpCode == 0xBF) Instruction = "dsubu";
            else if (OpCode == 0xC0) Instruction = "tge";
            else if (OpCode == 0xC1) Instruction = "tlt";
            else if (OpCode == 0xC2) Instruction = "teq";
            else if (OpCode == 0xC3) Instruction = "tne";
            else if (OpCode == 0xC4) Instruction = "tgeu";
            else if (OpCode == 0xC5) Instruction = "tltu";
            else if (OpCode == 0xC6) Instruction = "tge";
            else if (OpCode == 0xC7) Instruction = "tlt";
            else if (OpCode == 0xC8) Instruction = "teq";
            else if (OpCode == 0xC9) Instruction = "tne";
            else if (OpCode == 0xCA) Instruction = "tgeu";
            else if (OpCode == 0xCB) Instruction = "tltu";
            else if (OpCode == 0xCC) Instruction = "tge";
            else if (OpCode == 0xCD) Instruction = "tlt";
            else if (OpCode == 0xCE) Instruction = "teq";
            else if (OpCode == 0xCF) Instruction = "tne";
            else if (OpCode == 0xD0) Instruction = "tgeu";
            else if (OpCode == 0xD1) Instruction = "tltu";
            else if (OpCode == 0xD2) Instruction = "tge";
            else if (OpCode == 0xD3) Instruction = "tlt";
            else if (OpCode == 0xD4) Instruction = "teq";
            else if (OpCode == 0xD5) Instruction = "tne";
            else if (OpCode == 0xD6) Instruction = "tgeu";
            else if (OpCode == 0xD7) Instruction = "tltu";
            else if (OpCode == 0xD8) Instruction = "tge";
            else if (OpCode == 0xD9) Instruction = "tlt";
            else if (OpCode == 0xDA) Instruction = "teq";
            else if (OpCode == 0xDB) Instruction = "tne";
            else if (OpCode == 0xDC) Instruction = "tgeu";
            else if (OpCode == 0xDD) Instruction = "tltu";
            else if (OpCode == 0xDE) Instruction = "tge";
            else if (OpCode == 0xDF) Instruction = "tlt";
            else if (OpCode == 0xE0) Instruction = "teq";
            else if (OpCode == 0xE1) Instruction = "tne";
            else if (OpCode == 0xE2) Instruction = "tgeu";
            else if (OpCode == 0xE3) Instruction = "tltu";
            else if (OpCode == 0xE4) Instruction = "tge";
            else if (OpCode == 0xE5) Instruction = "tlt";
            else if (OpCode == 0xE6) Instruction = "teq";
            else if (OpCode == 0xE7) Instruction = "tne";
            else if (OpCode == 0xE8) Instruction = "tgeu";
            else if (OpCode == 0xE9) Instruction = "tltu";
            else if (OpCode == 0xEA) Instruction = "tge";
            else if (OpCode == 0xEB) Instruction = "tlt";
            else if (OpCode == 0xEC) Instruction = "teq";
            else if (OpCode == 0xED) Instruction = "tne";
            else if (OpCode == 0xEE) Instruction = "tgeu";
            else if (OpCode == 0xEF) Instruction = "tltu";
            else if (OpCode == 0xF0) Instruction = "tge";
            else if (OpCode == 0xF1) Instruction = "tlt";
            else if (OpCode == 0xF2) Instruction = "teq";
            else if (OpCode == 0xF3) Instruction = "tne";
            else if (OpCode == 0xF4) Instruction = "tgeu";
            else if (OpCode == 0xF5) Instruction = "tltu";
            else if (OpCode == 0xF6) Instruction = "tge";
            else if (OpCode == 0xF7) Instruction = "tlt";
            else if (OpCode == 0xF8) Instruction = "teq";
            else if (OpCode == 0xF9) Instruction = "tne";
            else if (OpCode == 0xFA) Instruction = "tgeu";
            else if (OpCode == 0xFB) Instruction = "tltu";
            else if (OpCode == 0xFC) Instruction = "tge";
            else if (OpCode == 0xFD) Instruction = "tlt";
            else if (OpCode == 0xFE) Instruction = "teq";
            else if (OpCode == 0xFF) Instruction = "tne";
            else Instruction = "Unknown Instruction";

            //Instruction Specifics
            switch (Instruction)
            {
                case "addi":
                case "addiu":
                case "andi":
                case "ori":
                case "xori":
                    DR = $"R{rt}"; // Destination register for I-type instructions
                    SR2 = immediate.ToString(); //Immediate Value
                    break;
                case "lw": //Offset(Rs) Rt - Memory[Rs + offset] -> Rt
                case "sw": //Offset(Rs) Rt - Rt -> Memory[Rs + offset]
                    DR = $"R{rt}";  //Rt is the data source
                    SR2 = immediate.ToString();  //Is the offset
                    break;
                case "beq":
                case "bne":
                    SR2 = immediate.ToString(); //Immediate is the offset
                    DR = ""; //No register to write back.
                    break;
                case "lui":
                    DR = $"R{rt}"; // Destination register for I-type instructions
                    SR1 = immediate.ToString();
                    SR2 = "";
                    break;

            }


        }
    }
}
