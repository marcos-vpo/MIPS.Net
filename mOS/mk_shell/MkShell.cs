using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.IOKit.Devices;
using mOS.IOKit.Structs;
using mOS.kernel;
using mOS.misc;
using mOS.process;

namespace mOS.mk_shell
{
    public class MkShell : ABIManaged
    {
        private IOConsoleService console;
        private string current_dir = "/";
        public MkShell()
        {
            console = new IOConsoleService(autoFlush: true);
        }
        internal void Start(bool welcome = true)
        {
            console.Clear();
            if (welcome)
            {

                console.Print("******************* ", foreground: mOSColor.Blue);
                console.Print("Welcome to ", foreground: mOSColor.White);
                console.Print("mOS ", foreground: mOSColor.Cyan);
                console.PrintLine("*******************", foreground: mOSColor.Blue);
                console.PrintLine($"                       v{typeof(MkShell).Assembly.GetName().Version} ", foreground: mOSColor.White);

                console.PrintLine("MIPS Operating System for MIPS Emulator Dotnet", foreground: mOSColor.White);

            }

            console.PrintLine("");
            console.PrintLine("This is MKShell");
            console.PrintLine("Type a command:");
            console.Print("# ");
            console.Flush();
             
            while (true)
            {
                string command = console.ReadLine();

                if (command == "ls") run_ls();
                else if (command == "cls" || command == "clear") run_clear();
                else if (command.StartsWith("run"))
                {
                    run_lauch_shell(command.Replace("run ", ""));
                    console.Dispose();
                    break;
                }
                else if (command == "exit")
                {
                    console.Dispose();
                    break;
                }
                else if (command == "proc")
                {
                    run_proc();
                }
                else if (command == "mem")
                {
                    run_mem();
                }
                console.Print("# ");
                console.Flush();
            }

        }

        private void run_mem()
        {
            var info = mos_kernel.PhysicalPageManager.GetInfo();
            console.Print($"Total:     ", foreground: mOSColor.Green);
            console.PrintLine($"{info.TotalMemory.FormatSize()}", foreground: mOSColor.White);

            console.Print($"Active:    ", foreground: mOSColor.Green);
            console.PrintLine($"{info.Active.FormatSize()}", foreground: mOSColor.White);

            console.Print($"Inactive:  ", foreground: mOSColor.Green);
            console.PrintLine($"{info.Inactive.FormatSize()}", foreground: mOSColor.White);

            console.Print($"Wired:     ", foreground: mOSColor.Green);
            console.PrintLine($"{info.Wired.FormatSize()}", foreground: mOSColor.White);
        }

        private void run_proc()
        {
            ProccessManager pm = mos_kernel.ProcessManager;
            ProcessInfo[] procs = pm.AllProcesses();

            foreach (ProcessInfo p in procs)
            {
                console.PrintLine($"PID: {p.pId} | Name: {p.ProcessName} | User: {p.User} | Mem: {p.memUsage} bytes ");
            }
        }

        private void run_lauch_shell(string file)
        {
            mos_kernel.ProcessManager.Launch("/applications/mOSShell/mShell.mex", "root");
        }

        private void run_clear()
        {
            console.Clear();
        }

        private void run_ls()
        {
            using (IOStorageService storage = new IOStorageService())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Directory_Info[] dirs = storage.ListDirectories(current_dir);
                sw.Stop();
                console.PrintLine($"TYPE      LAST_WRITE             SIZE         NAME");
                console.PrintLine($"+---------+----------------------+------------+-------------------");

                StringBuilder sb = new StringBuilder();
                foreach (Directory_Info di in dirs)
                {
                    sb.AppendLine($"[dir]     {DateTime.FromBinary(di.Last_Write)}    {($"{di.Size}".PadRight(7, ' '))}      {di.Name}");
//                    console.PrintLine($"[dir]     {DateTime.FromBinary(di.Last_Write)}    {($"{di.Size}".PadRight(7, ' '))}      {di.Name}");
                }

                console.PrintLine(sb.ToString());
                console.PrintLine($"\n{dirs.Length} directories reached in ~{sw.ElapsedMilliseconds}ms");
                console.PrintLine("\n");

                console.Flush();
            }
        }
    }
}
