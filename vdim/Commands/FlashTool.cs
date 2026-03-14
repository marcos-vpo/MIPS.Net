using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim.Commands
{
    internal class FlashTool : VCommand
    {
        private string baseSourceDir = "";
        public override void Run(string[] args)
        {
            string source = ReadString(args, 0, args.Length, "source directory (in your system): ");
            if (!Directory.Exists(source))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("directory not found");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            baseSourceDir = source;


            DirectoryInfo di = new DirectoryInfo(source);

            totalItems = di.GetFiles("*", SearchOption.AllDirectories).Length;
            totalItems += di.GetDirectories("*", SearchOption.AllDirectories).Length;

            FlashDirectory(di);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ok!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        double totalItems { get; set; }
        double flashedItems { get; set; }

        private void FlashDirectory(DirectoryInfo di)
        {
            string baseDir = di.FullName.Replace(baseSourceDir, "").Replace("\\", "/");
            //       if (string.IsNullOrEmpty(baseDir)) baseDir = "/";

            var d = Program.CurrentDisk;
            foreach (DirectoryInfo subDir in di.GetDirectories())
            {

                string sbd = $"{baseDir}/{subDir.Name}";

                Console.Write($"mkdir   {sbd}".PadRight(100, ' '));

                Stopwatch sw = Stopwatch.StartNew();
                d.CreateDir(sbd);
                sw.Stop();

                flashedItems += 1;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ok!  ~{sw.ElapsedMilliseconds}ms   {((flashedItems / totalItems) * 100):N2}%");
                Console.ForegroundColor = ConsoleColor.White;
                FlashDirectory(subDir);
            }

            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Name.Length > 47) continue;
                string fName = $"{baseDir}/{file.Name}";
                byte[] fData = File.ReadAllBytes(file.FullName);
                Stopwatch sw = null;
                try
                {
                    sw = Stopwatch.StartNew();
                    Console.Write($"{fName}".PadRight(100, ' '));
      
                    d.WriteFile(fName, fData);

                    sw.Stop();

                    flashedItems += 1;

                    var per = (flashedItems / totalItems) * 100;

                    if (flashedItems >= 500)
                    {

                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Ok!  ~{sw.ElapsedMilliseconds}ms   {per:N2}%");
                    Console.ForegroundColor = ConsoleColor.White;

                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"FAIL! ~{sw.ElapsedMilliseconds}ms   {((flashedItems / totalItems) * 100)}%");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
