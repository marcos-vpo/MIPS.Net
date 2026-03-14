using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim.Commands
{
    internal class Ls : VCommand
    {
        public override void Run(string[] args)
        {
            var disk = Program.CurrentDisk;
            List<VDIMLib.FileDirectory> dirs = disk.ListDir(Program.CurrentDirectory).ToList().OrderBy(x => x.Type).ToList();

            if(dirs == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return;
            }
            foreach (var dir in dirs)
            {
                if(dir.Type == "directory")
                    Console.WriteLine($"{($"/{dir.Name.PadRight(47, '.')} {dir.Size}")}  -- directory");
                else if (dir.Type == "file")
                    Console.WriteLine($"{($"{dir.Name.PadRight(47, '.')} {dir.Size}")}  -- file");
            }
        }
    }
}
