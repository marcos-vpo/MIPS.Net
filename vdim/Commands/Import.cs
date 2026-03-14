using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim.Commands
{
    internal class Import : VCommand
    {
        public override void Run(string[] args)
        {
            Console.Write("from: ");
            string? source = Console.ReadLine();
            if(!File.Exists(source))
            {
                Console.WriteLine($"file not found in your system: {source}");
                return;
            }

            Console.WriteLine();
            Console.Write($"to directory: ");
            string? destination = Console.ReadLine();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var d = Program.CurrentDisk;
            if(!d.DirectoryExists(destination))
            {
                sw.Stop();
                Console.WriteLine($"directory not found in disk: {destination}  ~{sw.ElapsedMilliseconds}ms");
                return;
            }

            FileInfo fi = new FileInfo(source);
            byte[] data = File.ReadAllBytes(source);

            d.WriteFile($"{destination}/{fi.Name}", data);

            sw.Stop();
            Console.WriteLine($"Import ok!  ~{sw.ElapsedMilliseconds}ms");
        }
    }
}
