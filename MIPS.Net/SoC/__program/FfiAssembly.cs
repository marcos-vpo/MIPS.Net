using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC.__program
{
    internal class FfiAssembly
    {
        public string Name { get; set; }    
        public Assembly Asm { get; set; }
        public AssemblyLoadContext Ctx { get; internal set; }
    }
}
