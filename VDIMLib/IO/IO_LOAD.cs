using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib.IO
{
    internal class IO_LOAD
    {
        public static void Load(FatDiskFile file)
        {
            file._entries = new List<FatEntry>();
            file._clusters = new List<FatCluster>();

            if (file._s.Length == 0) return;

            byte[] _csize = new byte[4];
            byte[] _cCout = new byte[4];
            byte[] _dLabel = new byte[10];
            byte[] _dSize = new byte[4];

            int entriesSizeArea = 960000;

            file._s.Read(_csize);
            file._s.Read(_cCout);
            file._s.Read(_dLabel);
            file._s.Read(_dSize);

            file.ClusterSize = BitConverter.ToInt32(_csize, 0);
            file.ClustersCount = BitConverter.ToInt32(_cCout, 0);
            file.DiskSize = BitConverter.ToInt32(_dSize, 0);
            file.DiskLabel = Encoding.UTF8.GetString(_dLabel);

            file._s.Position = _csize.Length + _cCout.Length + _dLabel.Length + _dSize.Length;
            for (int i = 0; i < (entriesSizeArea / 64); i++)
            {
                try
                {
                    var pos = file._s.Position;
                    byte[] e = new byte[64];
                    file._s.Read(e);

                    var dateTicks = BitConverter.ToInt64(new byte[8] { e[9], e[10], e[11], e[12], e[13], e[14], e[15], e[16] });

                    byte[] xName = new byte[47];
                    Array.Copy(e, 17, xName, 0, xName.Length);

                    FatEntry ftE = new FatEntry(
                        pos: pos,
                        type: (char)e[0],
                        parentId: BitConverter.ToInt32(new byte[4] { e[1], e[2], e[3], e[4], }),
                        entryId: BitConverter.ToInt32(new byte[4] { e[5], e[6], e[7], e[8], }),
                        creation: dateTicks == 0 ? null : new DateTime(dateTicks),
                        name: Encoding.UTF8.GetString(xName)
                    );

                    file._entries.Add(ftE);
                }
                catch(Exception ex)
                {

                }
            }

            // 640018
            int startLength = _csize.Length + _cCout.Length + _dLabel.Length + _dSize.Length + entriesSizeArea;

            file._s.Position = startLength;
            for (int i = 0; i < ((file.DiskSize - startLength) / file.ClusterSize) + 1; i++)
            {
                long start = file._s.Position;
                byte[] b = new byte[file.ClusterSize];
                file._s.Read(b);
                int entryId = BitConverter.ToInt32(new byte[4] { b[0], b[1], b[2], b[3] });
                int order = BitConverter.ToInt32(new byte[4] { b[4], b[5], b[6], b[7] });
                int dataLen = BitConverter.ToInt32(new byte[4] { b[8], b[9], b[10], b[11] });

                FatCluster c = new FatCluster(start, file._s.Position, dataLen, entryId, order);
                file._clusters.Add(c);
            }
        }
    }
}
