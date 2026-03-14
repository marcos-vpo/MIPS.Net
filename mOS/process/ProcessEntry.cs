using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.k_objects;
using mOS.misc;

namespace mOS.process
{
    internal class ProcessEntry : mOSObject
    {
        [FieldOrder(0)]
        public long UpTime { get; set; }
        [FieldOrder(1)]
        public byte Status { get; set; }
        [FieldOrder(2)]
        public long StatusTime { get; set; }
        [FieldOrder(3)]
        public string User { get; set; } = string.Empty;
        [FieldOrder(4)]
        public string WorkingDirectory { get; set; } = string.Empty;
        [FieldOrder(5)]
        public string ProcessName { get; set; } = string.Empty;
        

        // life-cycle
        [FieldOrder(6)]
        public int MainAddr { get; set; }  

    }
}
