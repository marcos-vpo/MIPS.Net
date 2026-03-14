using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class Cd : VCommand
    {
        public override void Run(string[] args)
        {
            string dir = ReadString(args, 0, args.Length, "");
            if (string.IsNullOrEmpty(dir)) return;


            var d = Program.CurrentDisk;

            if (dir == "..")
            {

                string[] crtDir = Program.CurrentDirectory.Split('/');
                string[] parentDir = new string[crtDir.Length - 1];
                Array.Copy(crtDir, 0, parentDir, 0, parentDir.Length);
                string dirStr = string.Join('/', parentDir);

                if (d.DirectoryExists(dirStr))
                {
                    Program.CurrentDirectory = $"{dirStr}";
                }
                else Console.WriteLine($"directory not found: '{dir}'");
            }
            else
            {
                if (!dir.StartsWith("/"))
                    dir = $"{Program.CurrentDirectory}/{dir}";

                if (d.DirectoryExists(dir))
                {

                    if (dir.StartsWith("/")) Program.CurrentDirectory = $"{dir}";
                    else
                    {
                        if (Program.CurrentDirectory == "/") Program.CurrentDirectory += $"{dir}";
                        else Program.CurrentDirectory += $"/{dir}";
                    }
                }
                else Console.WriteLine($"directory not found: '{dir}'");
            }
        }
    }
}
