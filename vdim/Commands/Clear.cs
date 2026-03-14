using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim.Commands
{
    internal class Clear : VCommand
    {
        public override void Run(string[] args)
        {
            try
            {
                Console.Clear();
            }
            catch { }
        }
    }
}
