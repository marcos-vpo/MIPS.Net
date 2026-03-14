using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim.Commands
{
    internal class MkDir : VCommand
    {
        public override void Run(string[] args)
        {
            string dir = ReadString(args, 0, args.Length, "dir name: ");

            var disk = Program.CurrentDisk;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!dir.StartsWith("/")) dir = $"{Program.CurrentDirectory}/{dir}";

            short res = disk.CreateDir(dir);
            sw.Stop();

            if (res == 1) Console.WriteLine($"directory created! ~{sw.ElapsedMilliseconds}ms");
            else Console.WriteLine($"directory already exist! ~{sw.ElapsedMilliseconds}ms");
        }
    }
}
