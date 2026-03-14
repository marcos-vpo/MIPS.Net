using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Compiler.ConsoleDebugger
{
    internal class ConsoleDbg
    {
        private MotherBoard fastBoard = null;
        public void Start()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("***    MIPS.NET Desktop  - Console Debugger   ***");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Loading machine...");

           
        }
    }
}
