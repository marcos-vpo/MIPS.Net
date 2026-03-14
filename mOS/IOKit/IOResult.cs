using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.k_objects;
using mOS.misc;

namespace mOS.IOKit
{
    internal class IOResult : mOSObject
    {
        [FieldOrder(0)]
        public bool IsComplete { get;set; }

        [FieldOrder(1)]
        public byte[] Data { get; set; }
    }
}
