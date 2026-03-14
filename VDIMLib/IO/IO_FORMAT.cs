using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib.IO
{
    internal class IO_FORMAT
    {
        public static void Format(FatDiskFile file, string baseFile, int clusterSize, int clustersCount, string diskLabel)
        {
            if (diskLabel.Length > 15)
                diskLabel = diskLabel.Substring(0, 15);

            if ( file._s.Length > 0)
            {
                file._s.Position = 0;
                byte[] clear = new byte[file._s.Length];
                file._s.Write(clear);
                file._s.Position = 0;
            }

            byte[] _csize = BitConverter.GetBytes(clusterSize);   // pos 0-3
            byte[] _cCout = BitConverter.GetBytes(clustersCount); // pos 4-8
            byte[] _dLabel = new byte[10];   // pos 8-18

            Array.Copy(Encoding.UTF8.GetBytes(diskLabel), _dLabel, diskLabel.Length);

            byte[] _dEntries = new byte[960000]; // pos 22-32000
            byte[] _dData = new byte[clusterSize * clustersCount];

            int diskSize = _csize.Length + _cCout.Length + _dLabel.Length + _dEntries.Length + _dData.Length;


            file._s.Write(_csize);  // 4
            file._s.Write(_cCout);  // 8
            file._s.Write(_dLabel); // 18
            file._s.Write(BitConverter.GetBytes(diskSize)); // 22
            file._s.Write(_dEntries);
            file._s.Write(_dData);

            file._s.SetLength(file._s.Position);
            file._s.Position = 0;
            //_s.Flush();
            //_s.Close();
            //_s.Dispose();
            //_s = null;
            //_s = new FileStream(baseFile, FileMode.OpenOrCreate);
        }
    }
}
