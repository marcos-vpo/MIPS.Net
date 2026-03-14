using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC.__program
{
    public class RotuleMapping
    {
        public override string ToString()
        {
            return $"[{Index}] : RELAT*[{RelativeAddr}]; ABSOL*[{AbsoluteAddr}]";
        }
        public int Index { get; set; }
        public int RelativeAddr { get; set; }
        public int AbsoluteAddr { get; set; }
        public int RotuleLength { get; set; }
    }
}
