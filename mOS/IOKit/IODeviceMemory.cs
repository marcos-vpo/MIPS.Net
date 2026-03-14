using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.k_objects;
using mOS.misc;

namespace mOS.IOKit
{
    internal class IODeviceMemory 
    {
        public int BaseDeviceAddr { get; set; }
        public byte Action { get; set; }
        public byte DeviceId { get; set; }
        public byte DeviceClass { get; set; }
        public byte Status { get; set; }
        public byte Operation { get; set; }
        public byte Mode { get; set; }
        public int BufferSize { get; set; }
        public int BufferAddr { get; set; }      // endereço na memória mapeada do firmware
        public long LastWrite { get;set; }
        public long LastRead { get; set; }
    }
}
