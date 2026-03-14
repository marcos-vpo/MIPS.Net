using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Desktop.ViewModel;
using Newtonsoft.Json;

namespace MIPS.Desktop.Utils
{
    internal class Systems
    {



        public static List<SystemVM> GetSystems()
        {
            string dirPath = @".\systems";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            List<SystemVM> res = new List<SystemVM>();

            DirectoryInfo systemsDir = new DirectoryInfo(dirPath);
            foreach (DirectoryInfo systemDir in systemsDir.GetDirectories())
            {
                string s_file_path = Path.Combine(systemDir.FullName, "system.json");
                if (!File.Exists(s_file_path)) continue;

                string json = File.ReadAllText(s_file_path);

                try
                {
                    SystemVM? s_vm = JsonConvert.DeserializeObject<SystemVM>(json);
                    if (s_vm == null) continue;
                    
                    s_vm.path = systemDir.FullName;
                    res.Add(s_vm);
                }
                catch { }
            }

            return res;
        }

        public static void CreateSystem(SystemVM vm)
        {
            string dirPath = $@".\systems\{vm.Name.Replace(" ", "_")}";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(Path.Combine(dirPath, "system.json"), json);
        }
    }
}
