using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class Mv : VCommand
    {
        public override void Run(string[] args)
        {
            string source = "";
            string destination = "";

            FatEntry? entry_source = null;
            FatEntry? entry_dest = null;

            var d = Program.CurrentDisk;
            if (args.Length == 0)
            {
                Console.WriteLine("origin file or directory: ");
                source = Console.ReadLine();
                if (string.IsNullOrEmpty(source)) { Console.WriteLine("cancelled"); return; }

                entry_source = d.GetEntryFor(source);
                if (entry_source == null) { Console.WriteLine($"origin file or directory not found: '{source}'"); return; }

                Console.WriteLine($"destination {(entry_source.Type == 'D' ? "directory" : "file")}: ");
                if (string.IsNullOrEmpty(destination)) { Console.WriteLine("cancelled"); return; }

                // check if destination *directory exists
                string[] dest_parts = destination.Split('/');
                string[] dest_dir_parts = new string[dest_parts.Length - 1];
                Array.Copy(dest_parts, 0, dest_dir_parts, 0, dest_parts.Length);

                string dest_directory = string.Join('/', dest_parts);

                entry_dest = d.GetEntryFor(dest_directory);
                if (entry_dest == null) { Console.WriteLine($"destination directory not found: '{dest_directory}'"); return; }

                d.ChangeParent(child: entry_source, newParent: entry_dest);
            }
            else
            {
                if (!args.Contains("-s"))
                {
                    Console.WriteLine("missing argument: -s  (source file or directory)");
                    return;
                }

                if (!args.Contains("-d"))
                {
                    Console.WriteLine("missing argument: -d  (destination file or directory)");
                    return;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                string fullArgs = string.Join(null, args);
                int len = fullArgs.Length;

                int s_start = fullArgs.IndexOf("-s");
                int s_len = fullArgs.IndexOf("-d") - 2;

                string sourceStr = fullArgs.Substring(s_start + 2, s_len);
                if (!sourceStr.StartsWith("/"))
                    sourceStr = $"{Program.CurrentDirectory}/{sourceStr}";

                int d_start = fullArgs.IndexOf("-d") + 2;
                int d_len = len - d_start;
                string destStr = fullArgs.Substring(d_start, d_len);

                entry_source = d.GetEntryFor(sourceStr);
                if (entry_source == null)
                {
                    sw.Stop();
                    Console.WriteLine($"origin file or directory not found: '{source}' ~{sw.ElapsedMilliseconds}ms");
                    return;
                }

                entry_dest = d.GetEntryFor(destStr);
                if (entry_dest == null)
                {
                    sw.Stop();
                    Console.WriteLine($"destination directory not found: '{destStr}' ~{sw.ElapsedMilliseconds}ms");
                    return;
                }

                if (entry_dest.Type == 'D')
                {
                    sw.Stop();
                    d.ChangeParent(entry_source, entry_dest);
                    Console.WriteLine($"Ok! ~{sw.ElapsedMilliseconds}ms");
                }
                else
                {
                    sw.Stop();
                    Console.WriteLine($"destination is not a directory: '{destStr}'; ~{sw.ElapsedMilliseconds}ms");
                }
            }
        }
    }
}
