using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MIPS.Net.SoC
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using MIPS.Net.InstructionSet;

    public class Registers
    {
        // Buffer linear fixo para eliminar buscas por Hash e garantir localidade no Cache L1/L2
        private readonly int[] _regArray;

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
        public const int FP = 0x0000001E;    // $fp  
        public const int RA = 0x0000001F;    // $ra
        public const int PC = 0x00000020;    // $pc

        public const int LO = 0x00000021;    // $lo
        public const int HI = 0x00000022;    // $hi

        public Registers()
        {
            // O maior código é 0x22 (HI). Alocamos 35 posições fixas.
            _regArray = new int[0x00000022 + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reset()
        {
            // Limpeza direta de memória em lote via Span limpa o array instantaneamente
            Array.Clear(_regArray, 0, _regArray.Length);
        }

        public int this[int registerCode]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // Acesso indexado O(1) puro. Força o JIT a embutir o método
                return _regArray[registerCode];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                // Proteção física do hardware MIPS: o registrador $zero nunca pode mudar de valor
                if (registerCode == 0) return;
                _regArray[registerCode] = value;
            }
        }

        public int this[string registerName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _regArray[CodeByName(registerName)];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int rCode = CodeByName(registerName);
                if (rCode == 0) return;
                _regArray[rCode] = value;
            }
        }

        // Sem alocação de strings extras. Switch-case com strings literais curtas é otimizado via Hashtable interna do JIT/IL
        public int CodeByName(string rn)
        {
            if (rn.Length < 3 && rn[0] != '$') rn = $"${rn}";

            switch (rn)
            {
                case "$zero": return 0x00000000;
                case "zero": return 0x00000000;
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
                case "$lo": return 0x00000021;
                case "$hi": return 0x00000022;

                default:
                    throw new ArgumentException($"Nome de registrador inválido: {rn}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void CopyTo(Registers before_interruption)
        {
            // Cópia linear de alta performance ignorando v0 e v1 conforme regra original
            for (int i = 0; i < _regArray.Length; i++)
            {
                if (i == 2 || i == 3) continue; // V0 e V1
                before_interruption._regArray[i] = _regArray[i];
            }
        }

        // Cache estático dos cabeçalhos das strings para zerar alocação de lixo no heap durante dumps
        private static readonly string[] _cachedNames = new string[]
        {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra",
            "$pc", "$lo", "$hi"
        };

        public string GetValues()
        {
            // Aloca uma única string builder baseada em Span sob o capô, evitando as mutações lentas de Split e Join antigos
            var sb = new StringBuilder(512);
            for (int i = 0; i < _regArray.Length; i++)
            {
                sb.Append(_cachedNames[i]).Append("  :  ").Append(_regArray[i]);
                if (i < _regArray.Length - 1) sb.Append('\n');
            }
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string NameByCode(int registerCode)
        {
            if (registerCode >= 0 && registerCode < _cachedNames.Length)
                return _cachedNames[registerCode];

            throw new ArgumentException("Código de registrador inválido: " + registerCode.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return _regArray.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string NameByIndex(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _regArray.Length)
                return "";

            return _cachedNames[rowIndex];
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