using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mBool : mOSObject
    {
        [FieldOrder(0)]
        public long Value { get; set; }

        public mBool()
        {

        }

        public mBool(long value)
        {
            this.Value = value;
        }
    }
}
