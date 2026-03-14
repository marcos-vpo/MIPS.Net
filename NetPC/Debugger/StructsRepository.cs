using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetPC.Debugger
{
    internal class StructsRepository
    {
        private static List<StructDefinition> _structs;
        private static bool init = false;
        public static void Add(StructDefinition sd)
        {
            if (init == false) Init();

            string json = JsonSerializer.Serialize(sd);

            string fName = Path.Combine(@".\debugger-structs", $"{sd.Name}.struct");
            File.WriteAllText(fName, json, Encoding.UTF8);

            var str = _structs.FirstOrDefault(s => s.Name == sd.Name);
            if (str != null) _structs.Remove(str);

            _structs.Add(sd);
        }

        public static void Remove(StructDefinition sd)
        {
            if (init == false) Init();
            var str = _structs.FirstOrDefault(s => s.Name == sd.Name);
            if (str != null) _structs.Remove(str);

            string fName = Path.Combine(@".\debugger-structs", $"{sd.Name}.struct");

            if (File.Exists(fName)) File.Delete(fName);
        }

        public static IReadOnlyCollection<StructDefinition> All()
        {
            if (init == false) Init();
            return _structs;
        }

        private static void Init()
        {
            string dir = @".\debugger-structs";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            _structs = new List<StructDefinition>();

            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo fi in di.GetFiles())
            {
                try
                {
                    string json = File.ReadAllText(fi.FullName);

                    StructDefinition? sd = System.Text.Json.JsonSerializer.Deserialize<StructDefinition>(json);

                    if (sd != null) _structs.Add(sd);
                }
                catch
                {

                }
            }

            init = true;
        }

        internal static StructDefinition? Get(string v)
        {
            if (init == false) Init();
            return _structs.FirstOrDefault(s => s.Name == v);
        }
    }
}
