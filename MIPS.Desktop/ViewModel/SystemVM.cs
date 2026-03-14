using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Desktop.ViewModel
{
    public class SystemVM
    {
        public string path;

        public string Name { get; set; }
        public string Vendor { get; set; }
        public int MemorySize { get; set; }
        public List<SystemPort> Ports { get; set; }
        public List<SystemButton> Buttons { get; set; }

        public SystemVM()
        {
            Ports = new List<SystemPort>();
            Buttons = new List<SystemButton>();
        }
    }
}
