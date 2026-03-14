using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Desktop.ViewModel
{
    public class SystemPort
    {
        public byte ID { get; set; }

        public string IDStr => $"0x{ID.ToString("X2")}";

        public byte Class { get; set; }

        public string ClassStr
        {
            get
            {
                if (Class == 0x20) return "USB (0x20)";
                if (Class == 0x30) return "SERIAL (0x30)";
                if (Class == 0x40) return "VGA (0x40)";
                return "???";
            }
        }
        public DeviceVM? Device { get; set; }
    }
}
