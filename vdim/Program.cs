

using vdim.Commands;
using VDIMLib;

namespace vdim
{
    internal class Program
    {
        public static IFatDisk CurrentDisk { get; set; }
        public static string CurrentDirectory { get; internal set; }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            if (args.Length == 0)
                args = new string[] { "open", "C:\\Temp\\mips_disk.img" };

            if (args.Length == 0)
                PrintWelcome();
            ProcessArgs(args);
        }

        private static void ProcessArgs(string[] args)
        {
            try
            {
                VCommand? cmd = GetCommand(args[0]);
                if (cmd == null) PrintWelcome();
                else
                {
                    string[] parArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, parArgs, 0, parArgs.Length);

                    cmd.Run(parArgs);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Command process fail: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static VCommand? GetCommand(string c)
        {
            switch (c)
            {
                case "create": return new DiskCreate();
                case "format": return new DiskFormat();
                case "open": return new DiskOpen();
                case "mkdir": return new MkDir();
                case "ls": return new Ls();
                case "cd": return new Cd();
                case "clear": return new Clear();
                case "cls": return new Clear();
                case "import": return new Import();
                case "export":  return new Export();
                case "rm": return new Rm();
                case "info": return new DiskInfo();
                case "mv": return new Mv();
                case "cp": return new Cp();
                case "flash": return new FlashTool();
                /* case "expand": return new CmdExpand();
       
               case "mount": return new CmdMount();
               case "umount": return new CmdUmount();
     
  


             
               case "touch": return new CmdTouch();
     */
                default: return null;
            }
        }

        static void PrintWelcome()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("============= Welcome to VDIM =============");
            Console.WriteLine("======== Virtual Disk Image Manager =======");
            Console.WriteLine("================= v 1.0.0 =================");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\nOptions List:");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[DISK OPERATIONS]");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("create ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[disk_label] [cluster_size] [cluster_count] [file_name]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - creates and format a virtual disk stored in a physical file \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("format ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[disk_label] [cluster_size] [cluster_count] [file_name]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - format a existing virtual disk stored in a physical file (this operation erases all data!) \n");
            Console.WriteLine(" - if have a openned disk, parameter [file_name] does not necessary \n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("expand ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[cluster_count] [file_name]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - expands a virtual disk adding more clusters (maybe a new fat table will be added to) \n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("open ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[file_name]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" - open virtual disk stored in a physical file \n    - you can run ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[I/O OPERATIONS]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" via CLI without the need to physically mount it \n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("mount ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[target_directory]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - physically mount the disk in your operating system and watch the mount directory \n    - any changes will be synchronized with the disk file \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("unmount ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - physically unmounts the current disk in your operating system\n    - pending changes will be safely synchronized and the directory will be cleaned up \n");


            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[I/O OPERATIONS]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[!] paths are separed by '/' \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("cd ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[directory]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - changes the current directory \n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("mkdir ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[directory]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - create a directory \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("rm ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[-r:recursively] [directory]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - remove a file or directory \n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("mv ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-s [source] -d [destination]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - move a file or directory \n ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("touch ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[file_name]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - create a empty file \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("import ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-s [source_file] -d [destination_file]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - import an external file frm your system into disk at a specified file path \n");


            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("export ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-s [source_file] -d [destination_file]");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[?]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - exports an internal file from virtual-disk file to your system at a specified file path \n");
        }
    }
}
