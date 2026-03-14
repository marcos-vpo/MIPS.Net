using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDIMLib;

namespace vdim.Commands
{
    internal class DiskInfo : VCommand
    {
        public static void PrintDiskInfo()
        {
            FatDiskFile d = (FatDiskFile)Program.CurrentDisk;
            Console.WriteLine($"Disk label:      [{d.DiskLabel}]");
            Console.WriteLine($"Cluster size:    [{d.ClusterSize} bytes]");
            Console.WriteLine($"Cluster's count: [{d.ClustersCount}] (~{Ext.HumanHeadableBytesString(d.ClustersCount * d.ClusterSize)} capacity)");

            var usedEntries = d.Entries.Where(e => e.EntryId > 0).ToList();
            var usedClusters = d.Clusters.Where(c => c.ENTRY > 0).ToList();


            Console.WriteLine($"Used entries:    [{usedEntries.Count} entries] (~{Ext.HumanHeadableBytesString(usedEntries.Count * 64)})");
            Console.WriteLine($"Used clusters:   [{usedClusters.Count} clusters] (~{Ext.HumanHeadableBytesString(usedClusters.Sum(c => c.LEN))})");
            Console.WriteLine($"Free space:      [~{Ext.HumanHeadableBytesString((d.ClustersCount * d.ClusterSize) - usedClusters.Sum(c => c.LEN))}]");

        }

        public override void Run(string[] args)
        {
            PrintDiskInfo();
        }
    }
}
