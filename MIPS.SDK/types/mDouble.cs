using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mDouble : mOSObject
    {
        [FieldOrder(0)]
        public double Value { get; set; }

        public mDouble()
        {

        }

        public mDouble(double value)
        {
            this.Value = value;
        }
    }
}
