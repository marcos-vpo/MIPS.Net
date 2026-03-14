using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class DiskFormat : VCommand
    {
        public override void Run(string[] args)
        {
            string label = ReadString(args, index: 0, Range: 1, msg: "Disk Label: ");
            int clusterSize = ReadInt(args, index: 1, msg: "Cluster Size: ");
            int clusterCount = ReadInt(args, index: 2, msg: "Clusters Count: ");
            string fileName = (Program.CurrentDisk == null
                    ? ReadString(args, index: 3, Range: args.Length - 3, msg: "Save to file: ")
                    : Program.CurrentDisk.FileName);

            if (!File.Exists(fileName)) throw new Exception("Specified disk file not exists");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            FatDiskFile file = (FatDiskFile)(Program.CurrentDisk == null
                ? new FatDiskFile(fileName)
                : Program.CurrentDisk);

            file.Format(clusterSize, clusterCount, label);

    

            sw.Stop();
            Console.WriteLine($"Format disk OK! ~{sw.ElapsedMilliseconds}ms");
        }
    }
}