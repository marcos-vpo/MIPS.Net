using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib.IO
{
    internal class IO_CREATE_DIRECTORY
    {
        public static short CreateDir(FileStream _s, IReadOnlyCollection<FatEntry> _entries, string path)
        {
            string[] parts = path.Split('/');
            parts[0] = "/";
            int[] entryIds = new int[parts.Length];

            for (var p = 0; p < parts.Length; p++)
            {
                FatEntry? unused = (p == 0
                    ? _entries.FirstOrDefault(e => e.Name == parts[p])
                    : _entries.FirstOrDefault(e => e.Name == parts[p] && e.ParentId == entryIds[p - 1]));

                if (unused == null) //to write
                {
                    unused = _entries.FirstOrDefault(e => e.EntryId == 0);
                    int entryId = _entries.Max(e => e.EntryId) + 1;
                    byte[] b = new byte[64];
                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        byte[] xNameRaw = Encoding.ASCII.GetBytes(parts[p]);
                        byte[] xName = new byte[47];

                        if (xNameRaw.Length > 47)
                            Array.Copy(xNameRaw, xName, xNameRaw.Length);
                        else
                            Array.Copy(xNameRaw, xName, xNameRaw.Length);


                        ms.Write("D"u8); // type 0
                        ms.Write(BitConverter.GetBytes(p == 0 ? 0 : entryIds[p - 1])); // parent id 1-4
                        ms.Write(BitConverter.GetBytes(entryId)); // entry id 5-8
                        ms.Write(BitConverter.GetBytes(DateTime.Now.Ticks)); // last-change 9-16
                        ms.Write(xName); // name 17-64
                    }

                    entryIds[p] = entryId;

                    _s.Position = unused.EntryPos;
                    _s.Write(b);
                    _s.Flush();

                    unused.Update('D',
                            parentId: p == 0 ? 0 : entryIds[p - 1],
                            entryId: entryId,
                            name: parts[p]);

                    return 1;
                }
                else entryIds[p] = unused.EntryId;
            }

            return 0;
        }
    }
}
