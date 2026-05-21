using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.heap;

namespace mOSLib.types
{
    public class mFloat : mOSObject
    {
        [FieldOrder(0)]
        public float Value { get; set; }

        public mFloat()
        {

        }

        public mFloat(float value)
        {
            this.Value = value;
        }
    }
}
