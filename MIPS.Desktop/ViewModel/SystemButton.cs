using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Desktop.ViewModel
{
    public class SystemButton
    {
        public byte ID { get; set; }
        public string IDStr => $"0x{ID.ToString("X2")}";
        public string Text { get; set; }
        public int IntrClick { get; set; }
    }
}
