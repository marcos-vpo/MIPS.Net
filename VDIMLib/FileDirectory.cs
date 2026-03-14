using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib
{
    public class FileDirectory
    {
        public override string ToString()
        {
            return $"[{Type[0]}] {Name} {Size}";
        }
        public object Icon { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
    }
}
