using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mShort: mOSObject
    {
        [FieldOrder(0)]
        public short Value { get; set; }

        public mShort()
        {

        }

        public mShort(short value)
        {
            this.Value = value;
        }
    }
}
