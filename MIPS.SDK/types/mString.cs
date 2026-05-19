using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mString : mOSObject
    {
        [FieldOrder(0)]
        public string Value { get; set; }

        public mString()
        {
            
        }

        public mString(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

    }
}
