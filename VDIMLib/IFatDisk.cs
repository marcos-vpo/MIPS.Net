using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib
{
    public interface IFatDisk
    {
        int ClusterSize { get;  }
        int ClustersCount { get; }
        int DiskSize { get; }
        string DiskLabel { get; }
        IReadOnlyCollection<FatEntry> Entries { get; }
        IReadOnlyCollection<FatCluster> Clusters { get; }
        string FileName { get; }

        void Format(int clusterSize, int clustersCount, string diskLabel);

        short CreateDir(string path);

        short RemoveDir(string path);

        short WriteFile(string path, byte[] data);
        short RemoveFile(string path);
        byte[]  ReadFile(string path);
        void Load();
        IReadOnlyCollection<FileDirectory> ListDir(string path);
        bool DirectoryExists(string dir);
        FatEntry? GetEntryFor(string fileOrDir);
        bool HasChilds(FatEntry e);
        FatEntry? GetParent(FatEntry e);
        short RemoveFile(FatEntry? e);
        short RemoveDir(FatEntry e);
        short ChangeParent(FatEntry child, FatEntry newParent);
    }
}
