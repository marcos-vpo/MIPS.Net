using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.misc
{
    internal static class ByteLenFormat
    {
        public static string FormatSize(this int bytes)
        {
            if (bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            const double KB = 1024;
            const double MB = KB * 1024;
            const double GB = MB * 1024;

            if (bytes < KB)
                return $"{bytes} B";
            if (bytes < MB)
                return $"{bytes / KB:0.##} KB";
            if (bytes < GB)
                return $"{bytes / MB:0.##} MB";

            return $"{bytes / GB:0.##} GB";
        }

    }
}
