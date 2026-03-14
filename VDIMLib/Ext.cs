using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib
{
    public static class Ext
    {
        public static String HumanHeadableBytesString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        public static decimal CustomParseDecimal(this string input)
        {
            long n = 0;
            int decimalPosition = input.Length;
            for (int k = 0; k < input.Length; k++)
            {
                char c = input[k];
                if (c == '.')
                    decimalPosition = k + 1;
                else
                    n = (n * 10) + (int)(c - '0');
            }
            return new decimal((int)n, (int)(n >> 32), 0, false, (byte)(input.Length - decimalPosition));
        }

        public static string[] ToCPUStr(this string str)
        {
            byte[] bytes = new byte[str.Length];
            for(int c = 0; c < str.Length; c++)
                bytes[c] = (byte)str[c];
    
            string[] res = Gen(bytes);
            return res;
        }

        public static string GenStr(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                var step1 = Convert.ToString(b, 2);
                var step2 = step1.PadLeft(8, '0');
                string bit = step2;

                sb.Append($"{bit} ");
            }
            return sb.ToString().Trim();
        }

        public static string[] Gen(byte[] bytes)
        {
            string[] res = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                var step1 = Convert.ToString(b, 2);
                var step2 = step1.PadLeft(8, '0');
                string bit = step2;

                res[i] = bit;
            }
            return res;
        }
    }
}
