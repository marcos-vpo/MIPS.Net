using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mByte
    {
        [FieldOrder(0)]
        public byte Value { get; set; }

        public mByte()
        {

        }

        public mByte(byte value)
        {
            this.Value = value;
        }
    }
}
