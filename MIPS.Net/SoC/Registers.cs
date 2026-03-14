using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using MIPS.Net.InstructionSet;

    public class Registers
    {
        private Dictionary<int, int> _registers;
        public const int ZERO = 0x00000000;  // $zero
        public const int AT = 0x00000001;    // $at
        public const int V0 = 0x00000002;    // $v0
        public const int V1 = 0x00000003;    // $v1
        public const int A0 = 0x00000004;    // $a0
        public const int A1 = 0x00000005;    // $a1
        public const int A2 = 0x00000006;    // $a2
        public const int A3 = 0x00000007;    // $a3
        public const int T0 = 0x00000008;    // $t0
        public const int T1 = 0x00000009;    // $t1
        public const int T2 = 0x0000000A;    // $t2
        public const int T3 = 0x0000000B;    // $t3
        public const int T4 = 0x0000000C;    // $t4
        public const int T5 = 0x0000000D;    // $t5
        public const int T6 = 0x0000000E;    // $t6
        public const int T7 = 0x0000000F;    // $t7
        public const int S0 = 0x00000010;    // $s0
        public const int S1 = 0x00000011;    // $s1
        public const int S2 = 0x00000012;    // $s2
        public const int S3 = 0x00000013;    // $s3
        public const int S4 = 0x00000014;    // $s4
        public const int S5 = 0x00000015;    // $s5
        public const int S6 = 0x00000016;    // $s6
        public const int S7 = 0x00000017;    // $s7
        public const int T8 = 0x00000018;    // $t8
        public const int T9 = 0x00000019;    // $t9
        public const int K0 = 0x0000001A;    // $k0
        public const int K1 = 0x0000001B;    // $k1
        public const int GP = 0x0000001C;    // $gp
        public const int SP = 0x0000001D;    // $sp
        public const int FP = 0x0000001E;    // $fp  (Também conhecido como $s8)
        public const int RA = 0x0000001F;    // $ra
        public const int PC = 0x00000020;    // $pc

        public const int LO = 0x00000021;    // $lo
        public const int HI = 0x00000022;    // $hi
        public Registers()
        {
            _registers = new Dictionary<int, int>()
        {
            { 0x00000000, 0 }, // $zero
            { 0x00000001, 0 }, // $at
            { 0x00000002, 0 }, // $v0
            { 0x00000003, 0 }, // $v1
            { 0x00000004, 0 }, // $a0
            { 0x00000005, 0 }, // $a1
            { 0x00000006, 0 }, // $a2
            { 0x00000007, 0 }, // $a3
            { 0x00000008, 0 }, // $t0
            { 0x00000009, 0 }, // $t1
            { 0x0000000A, 0 }, // $t2
            { 0x0000000B, 0 }, // $t3
            { 0x0000000C, 0 }, // $t4
            { 0x0000000D, 0 }, // $t5
            { 0x0000000E, 0 }, // $t6
            { 0x0000000F, 0 }, // $t7
            { 0x00000010, 0 }, // $s0
            { 0x00000011, 0 }, // $s1
            { 0x00000012, 0 }, // $s2
            { 0x00000013, 0 }, // $s3
            { 0x00000014, 0 }, // $s4
            { 0x00000015, 0 }, // $s5
            { 0x00000016, 0 }, // $s6
            { 0x00000017, 0 }, // $s7
            { 0x00000018, 0 }, // $t8
            { 0x00000019, 0 }, // $t9
            { 0x0000001A, 0 }, // $k0
            { 0x0000001B, 0 }, // $k1
            { 0x0000001C, 0 }, // $gp
            { 0x0000001D, 0 }, // $sp
            { 0x0000001E, 0 }, // $fp
            { 0x0000001F, 0 }, // $ra,
            { 0x00000020, 0 }, // $pc 
            { 0x00000021, 0 },
            { 0x00000022, 0 }
    };
        }


        internal void Reset()
        {
            foreach (var reg in _registers)
                _registers[reg.Key] = 0;
        }

        public int this[int registerCode]
        {
            get
            {
                return _registers[registerCode];
            }
            set
            {
                _registers[registerCode] = value;
            }
        }

        public int this[string registerName]
        {
            get
            {
                int rCode = CodeByName(registerName);
                return this[rCode];
            }
            set
            {
                int rCode = CodeByName(registerName);
                _registers[rCode] = value;
            }
        }

        public int CodeByName(string rn)
        {
            if (rn.Length < 3) rn = $"${rn}";
            //        byte[] b = new byte[3] { (byte)rn[0], (byte)rn[1], (byte)rn[2] };

            // Converte o nome do registrador para código
            switch (rn)
            {
                case "$at": return 0x00000001;
                case "$v0": return 0x00000002;
                case "$v1": return 0x00000003;
                case "$a0": return 0x00000004;
                case "$a1": return 0x00000005;
                case "$a2": return 0x00000006;
                case "$a3": return 0x00000007;
                case "$t0": return 0x00000008;
                case "$t1": return 0x00000009;
                case "$t2": return 0x0000000A;
                case "$t3": return 0x0000000B;
                case "$t4": return 0x0000000C;
                case "$t5": return 0x0000000D;
                case "$t6": return 0x0000000E;
                case "$t7": return 0x0000000F;
                case "$s0": return 0x00000010;
                case "$s1": return 0x00000011;
                case "$s2": return 0x00000012;
                case "$s3": return 0x00000013;
                case "$s4": return 0x00000014;
                case "$s5": return 0x00000015;
                case "$s6": return 0x00000016;
                case "$s7": return 0x00000017;
                case "$t8": return 0x00000018;
                case "$t9": return 0x00000019;
                case "$k0": return 0x0000001A;
                case "$k1": return 0x0000001B;
                case "$gp": return 0x0000001C;
                case "$sp": return 0x0000001D;
                case "$fp": return 0x0000001E;
                case "$ra": return 0x0000001F;
                case "$pc": return 0x00000020;

                default:
                    {
                        if (rn == "$zero" || rn == "zero") return 0x00000000;
                        throw new ArgumentException($"Nome de registrador inválido: {rn}");
                    }
            }
        }

        internal void CopyTo(Registers before_interruption)
        {
            foreach (var key in _registers)
            {
                if (key.Key == Registers.V0 || key.Key == Registers.V1) continue;
                before_interruption[key.Key] = key.Value;
            }
        }

        string[] regs = null;
        public string GetValues()
        {

            if (regs == null)
            {
                regs = new string[33];
                int ind = 0;
                foreach (var kvp in _registers)
                {
                    string registerName = NameByCode(kvp.Key);

                    regs[ind] = $"{registerName}  :  {kvp.Value}";
                    ind += 1;
                    if (ind == 33)
                        break;
                }
            }
            else
            {
                regs[0] = $"{regs[0].Split(" : ")[0]} : {_registers.ElementAt(0).Value}";
                regs[1] = $"{regs[1].Split(" : ")[0]} : {_registers.ElementAt(1).Value}";
                regs[2] = $"{regs[2].Split(" : ")[0]} : {_registers.ElementAt(2).Value}";
                regs[3] = $"{regs[3].Split(" : ")[0]} : {_registers.ElementAt(3).Value}";
                regs[4] = $"{regs[4].Split(" : ")[0]} : {_registers.ElementAt(4).Value}";
                regs[5] = $"{regs[5].Split(" : ")[0]} : {_registers.ElementAt(5).Value}";
                regs[6] = $"{regs[6].Split(" : ")[0]} : {_registers.ElementAt(6).Value}";
                regs[7] = $"{regs[7].Split(" : ")[0]} : {_registers.ElementAt(7).Value}";
                regs[8] = $"{regs[8].Split(" : ")[0]} : {_registers.ElementAt(8).Value}";
                regs[9] = $"{regs[9].Split(" : ")[0]} : {_registers.ElementAt(9).Value}";
                regs[10] = $"{regs[10].Split(" : ")[0]} : {_registers.ElementAt(10).Value}";
                regs[11] = $"{regs[11].Split(" : ")[0]} : {_registers.ElementAt(11).Value}";
                regs[12] = $"{regs[12].Split(" : ")[0]} : {_registers.ElementAt(12).Value}";
                regs[13] = $"{regs[13].Split(" : ")[0]} : {_registers.ElementAt(13).Value}";
                regs[14] = $"{regs[14].Split(" : ")[0]} : {_registers.ElementAt(14).Value}";
                regs[15] = $"{regs[15].Split(" : ")[0]} : {_registers.ElementAt(15).Value}";
                regs[16] = $"{regs[16].Split(" : ")[0]} : {_registers.ElementAt(16).Value}";
                regs[17] = $"{regs[17].Split(" : ")[0]} : {_registers.ElementAt(17).Value}";
                regs[18] = $"{regs[18].Split(" : ")[0]} : {_registers.ElementAt(18).Value}";
                regs[19] = $"{regs[19].Split(" : ")[0]} : {_registers.ElementAt(19).Value}";
                regs[20] = $"{regs[20].Split(" : ")[0]} : {_registers.ElementAt(20).Value}";
                regs[21] = $"{regs[21].Split(" : ")[0]} : {_registers.ElementAt(21).Value}";
                regs[22] = $"{regs[22].Split(" : ")[0]} : {_registers.ElementAt(22).Value}";
                regs[23] = $"{regs[23].Split(" : ")[0]} : {_registers.ElementAt(23).Value}";
                regs[24] = $"{regs[24].Split(" : ")[0]} : {_registers.ElementAt(24).Value}";
                regs[25] = $"{regs[25].Split(" : ")[0]} : {_registers.ElementAt(25).Value}";
                regs[26] = $"{regs[26].Split(" : ")[0]} : {_registers.ElementAt(26).Value}";
                regs[27] = $"{regs[27].Split(" : ")[0]} : {_registers.ElementAt(27).Value}";
                regs[28] = $"{regs[28].Split(" : ")[0]} : {_registers.ElementAt(28).Value}";
                regs[29] = $"{regs[29].Split(" : ")[0]} : {_registers.ElementAt(29).Value}";
                regs[30] = $"{regs[30].Split(" : ")[0]} : {_registers.ElementAt(30).Value}";
                regs[31] = $"{regs[31].Split(" : ")[0]} : {_registers.ElementAt(31).Value}";
                regs[32] = $"{regs[32].Split(" : ")[0]} : {_registers.ElementAt(32).Value}";

            }

            return string.Join("\n", regs);
        }

        private string NameByCode(int registerCode)
        {
            switch (registerCode)
            {
                case 0x00000000: return "$zero";
                case 0x00000001: return "$at";
                case 0x00000002: return "$v0";
                case 0x00000003: return "$v1";
                case 0x00000004: return "$a0";
                case 0x00000005: return "$a1";
                case 0x00000006: return "$a2";
                case 0x00000007: return "$a3";
                case 0x00000008: return "$t0";
                case 0x00000009: return "$t1";
                case 0x0000000A: return "$t2";
                case 0x0000000B: return "$t3";
                case 0x0000000C: return "$t4";
                case 0x0000000D: return "$t5";
                case 0x0000000E: return "$t6";
                case 0x0000000F: return "$t7";
                case 0x00000010: return "$s0";
                case 0x00000011: return "$s1";
                case 0x00000012: return "$s2";
                case 0x00000013: return "$s3";
                case 0x00000014: return "$s4";
                case 0x00000015: return "$s5";
                case 0x00000016: return "$s6";
                case 0x00000017: return "$s7";
                case 0x00000018: return "$t8";
                case 0x00000019: return "$t9";
                case 0x0000001A: return "$k0";
                case 0x0000001B: return "$k1";
                case 0x0000001C: return "$gp";
                case 0x0000001D: return "$sp";
                case 0x0000001E: return "$fp";
                case 0x0000001F: return "$ra";
                case 0x00000020: return "$pc";
                default: throw new ArgumentException("Código de registrador inválido: "  + registerCode.ToString());
            }
        }

        public int Count()
        {
            return _registers.Count;
        }

        public string NameByIndex(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _registers.Count)
                return "";

            int registerCode = _registers.Keys.ElementAt(rowIndex);
            return NameByCode(registerCode);
        }
    }

    public class RegisterView
    {
        public string Key { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{Key} : {Value}";
        }

        public RegisterView(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}
