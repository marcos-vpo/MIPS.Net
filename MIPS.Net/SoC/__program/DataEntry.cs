using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC.__program
{
    public class DataEntry
    {

        public override string ToString()
        {
            return $"  [{Index}]: {Type} ({Data.Length}b) at *{Address}  ";
        }
        public int Index { get; set; }
        public int Address { get; set; }
        public DataType Type { get; set; }
        public byte[] Data { get; set; }

    }
}
