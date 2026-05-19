using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPC.DTO
{
    public class MachineDTO
    {
        public string Name { get; set; }
        public string FirmwareLocation { get;set; }
        public int TotalMemory { get; set; }  
        
        // "B" / "MB"
        public string MemoryUnit { get; set; }
    
        // external device port Id's
        public byte[] Ports { get; set; }

        // hardware button Id's
        public byte[] Buttons { get;set;}
    }

    public class MachinePortDTO
    {
        public byte HWID { get; set; }
        public string Description { get; set; } 
    }

    public class MachineButtonDTO
    {
        public byte HWID { get; set; }
        public string Description { get; set; }
        public byte[] Icon { get; set; }
    }
}
