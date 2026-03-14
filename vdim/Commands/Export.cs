using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class Export : VCommand
    {
        public override void Run(string[] args)
        {
            Console.Write("from: ");
            string? source = Console.ReadLine();

            var d = Program.CurrentDisk;

            FatEntry? e = d.GetEntryFor(source);
            if (e == null)
            {
                Console.WriteLine($"file not found in disk: {source}");
                return;
            }

            if(e.Type == 'D')
            {
                Console.WriteLine($"{source} is a directory");
                return;
            }

            Console.WriteLine();
            Console.Write($"to your system directory: ");
            string? destination = Console.ReadLine();
            if (!Directory.Exists(destination))
            {
                Console.WriteLine($"directory not found in your system: {source}");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] data = d.ReadFile(source);
            File.WriteAllBytes(Path.Combine(destination, e.Name), data);
        }
    }
}
