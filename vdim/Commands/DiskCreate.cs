using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class DiskCreate : VCommand
    {
        public override void Run(string[] args)
        {
            string label = ReadString(args, index: 0, Range: 1, msg: "Disk Label: ");
            int clusterSize = ReadInt(args, index: 1, msg: "Cluster Size: ");
            int clusterCount = ReadInt(args, index: 2, msg: "Clusters Count: ");
            string fileName = ReadString(args, index: 3, Range: args.Length - 3, msg: "Save to file: ");

            if (File.Exists(fileName)) throw new Exception("Specified disk file already exists");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            FatDiskFile file = new FatDiskFile(fileName);
            file.Format(clusterSize, clusterCount, label);
            file.Close();

            sw.Stop();
            Console.WriteLine($"Create disk OK! ~{sw.ElapsedMilliseconds}ms");
        }

    }
}
