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
    internal class Devices
    {
        public static List<DeviceVM> GetDevices()
        {
            string dirPath = @".\devices";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            List<DeviceVM> res = new List<DeviceVM>();

            DirectoryInfo devicesDir = new DirectoryInfo(dirPath);
            foreach (FileInfo devFile in devicesDir.GetFiles())
            {
                string json = File.ReadAllText(devFile.FullName);

                try
                {
                    DeviceVM? dv_vm = JsonConvert.DeserializeObject<DeviceVM>(json);
                    if (dv_vm == null) continue;

                    dv_vm.path = devFile.FullName;
                    res.Add(dv_vm);
                }
                catch { }
            }

            return res;
        }

        public static void CreateDevice(DeviceVM vm)
        {
            if (string.IsNullOrEmpty(vm.Name)) throw new Exception("Device name is required");
            if (vm.Name.Length < 4) throw new Exception("Device name is too short");
            if (string.IsNullOrEmpty(vm.ExePath)) throw new Exception("An external program is required");
            if(!File.Exists(vm.ExePath)) throw new Exception("An external valid and existing program is required");

            string dirPath = $@".\devices";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string json = JsonConvert.SerializeObject(vm, Formatting.Indented);
            File.WriteAllText(Path.Combine(dirPath, $"{vm.Name.Replace(" ", "_")}.json"), json);
        }
    }
}
