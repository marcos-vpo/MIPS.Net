using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mLong : mOSObject
    {
        [FieldOrder(0)]
        public long Value { get; set; }

        public mLong()
        {

        }

        public mLong(long value)
        {
            this.Value = value;
        }
    }
}
