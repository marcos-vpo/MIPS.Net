using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mByteArray : mOSObject
    {
        [FieldOrder(0)]
        public byte[] Value { get; set; }

        public mByteArray()
        {

        }

        public mByteArray(byte[] value)
        {
            this.Value = value;
        } 
    }
}
