using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class Rm : VCommand
    {
        public override void Run(string[] args)
        {
            string fileOrDir = ReadString(args, 0, args.Length, "file or directory: ");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!fileOrDir.StartsWith("/")) fileOrDir = $"{Program.CurrentDirectory}/{fileOrDir}";

            IFatDisk d = Program.CurrentDisk;
            FatEntry? e = d.GetEntryFor(fileOrDir);
            if (e == null)
                Console.Write($"file or directory not found '{fileOrDir}'");
            else
            {
                if (e.Type == 'F')
                {
                    d.RemoveFile(e);

                }
                else
                {
                    d.RemoveDir(e);
                }
            }

            sw.Stop();
            Console.WriteLine($"Ok! ~{sw.ElapsedMilliseconds}ms");
        }

        private void DeleteChilds(FatEntry e)
        {
            throw new NotImplementedException();
        }
    }
}
