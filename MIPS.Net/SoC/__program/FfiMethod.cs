using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC.__program
{
    internal class FfiMethod
    {
        public string CallingName { get; set; }

        private int _asmIndex = 0;
        public int AssemblyIndex
        {
            get => _asmIndex; 
            set
            {
                _asmIndex = value;
                Linked = true;
            }
        }
        public bool Linked { get; private set; }
    }
}
