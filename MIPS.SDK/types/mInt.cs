using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mInt : mOSObject
    {
        [FieldOrder(0)]
        public int Value { get; set; }

        public mInt()
        {

        }

        public mInt(int value)
        {
            this.Value = value;
        }
    }
}
