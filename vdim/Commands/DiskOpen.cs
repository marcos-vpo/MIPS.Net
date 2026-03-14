using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VDIMLib;

namespace vdim.Commands
{
    internal class DiskOpen : VCommand
    {

        public override void Run(string[] args)
        {
            string fileName = ReadString(args, index: 0, Range: args.Length, msg: "Open disk file: ");

            if (!File.Exists(fileName)) throw new Exception("Specified disk file not exists");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Program.CurrentDisk = new FatDiskFile(fileName);
            Program.CurrentDisk.Load();
            Program.CurrentDirectory = "/";
            sw.Stop();
            Console.WriteLine($"Disk open! ~{sw.ElapsedMilliseconds}ms");

            DiskInteractive();
        }

    

        bool wellcome = false;

        private void DiskInteractive()
        {

            if (!wellcome) DiskInfo.PrintDiskInfo();
            wellcome = true;

            var d = Program.CurrentDisk;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write($"{d.DiskLabel}@{new FileInfo(d.FileName).Name}:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{Program.CurrentDirectory}$ ");
            Console.ForegroundColor = ConsoleColor.White;

            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                DiskInteractive();
                return;
            }

            string[] args = line.Split(' ');

            VCommand? cmd = Program.GetCommand(args[0]);
            if(cmd == null)
            {
                Console.WriteLine($"command '{args[0]}' not found");
                DiskInteractive();
                return;
            }

            string[] parArgs = new string[args.Length - 1];
            Array.Copy(args, 1, parArgs, 0, parArgs.Length);

            cmd.Run(parArgs);

            DiskInteractive();
        }
    }
}