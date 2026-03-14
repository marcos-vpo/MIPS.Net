using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOSLib
{
    internal class ProcessHeapPage
    {
        public ProcessHeapPage(ref byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length < 16)
                throw new ArgumentException("Buffer must contain at least 16 bytes", nameof(buffer));

            ProcessId = BitConverter.ToInt32(buffer, 0);
            Order = BitConverter.ToInt32(buffer, 4);
            PageIndex = BitConverter.ToInt32(buffer, 8);
            Usage = BitConverter.ToInt32(buffer, 12);
        }

        public int ProcessId { get; private set; }
        public int Order { get; private set; }
        public int PageIndex { get; private set; }
        public int Usage { get; private set; }
    }
}
