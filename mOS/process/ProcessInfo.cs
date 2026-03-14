using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.misc;

namespace mOS.process
{
    public class ProcessInfo
    {
        public int pId { get; set; }
        public int memUsage { get; set; }

        public long UpTime { get; set; }
        public byte Status { get; set; }
        public long StatusTime { get; set; }
        public string User { get; set; } = string.Empty;
        public string WorkingDirectory { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int MainAddr { get; set; }
    }
}
