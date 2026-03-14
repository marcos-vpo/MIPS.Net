using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Compiler
{
    internal class Utils
    {
        public static uint Fnv1a(string text)
        {
            const uint offset = 2166136261;
            const uint prime = 16777619;

            uint hash = offset;
            foreach (byte b in Encoding.UTF8.GetBytes(text))
            {
                hash ^= b;
                hash *= prime;
            }

            return hash;
        }
    }
}
