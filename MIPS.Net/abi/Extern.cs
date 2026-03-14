using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Abi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Extern : Attribute
    {
    }
}
