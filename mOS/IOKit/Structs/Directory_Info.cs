using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.k_objects;

namespace mOS.IOKit.Structs
{
    internal class Directory_Info : mOSObject
    {
        // dir tuple: [ creation (8b) + last_change (8b) + files (4b) + size (4b) + name (Nb....\0) ]
        public long Creation { get; set; }
        public long Last_Write { get;set; }
        public long Last_Access { get; set; }
        public int Files { get; set; }
        public long Size { get; set; }
        public string Name { get; set; } = "";


        public override string ToString()
        {
            return $"{Name} [{Files} files, {Size} bytes]";
        }

    }
}
