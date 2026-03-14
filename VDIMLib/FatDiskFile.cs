using static System.Net.Mime.MediaTypeNames;
using System.Resources;
using System.Text;
using VDIMLib.IO;
using System.IO;

namespace VDIMLib
{
    public class FatDiskFile : IFatDisk
    {
        public int ClusterSize { get; internal set; }
        public int ClustersCount { get; internal set; }
        public int DiskSize { get; internal set; }
        public string DiskLabel { get; internal set; }


        internal FileStream _s;

        public FatDiskFile(string baseFile)
        {
            this.baseFile = baseFile;
            _s = new FileStream(baseFile, FileMode.OpenOrCreate);

        }

        public IReadOnlyCollection<FatEntry> Entries => _entries;
        public IReadOnlyCollection<FatCluster> Clusters => _clusters;

        public string FileName => this.baseFile;

        internal List<FatCluster> _clusters = new List<FatCluster>();
        internal List<FatEntry> _entries = new List<FatEntry>();
        internal readonly string baseFile;

        public void Load()
        {
            IO_LOAD.Load(this);
        }


        public void Close()
        {
            try
            {
                _s.Flush();
                _s.Close();
                _s.Dispose();
            }
            catch
            {

            }
        }

        public void Format(int clusterSize, int clustersCount, string diskLabel)
        {
            diskLabel = diskLabel.Replace(" ", "_");
            IO_FORMAT.Format(this, baseFile, clusterSize, clustersCount, diskLabel);
            Close();

            _s = new FileStream(baseFile, FileMode.OpenOrCreate);

            Load();
            CreateDir("/");
        }

        public short CreateDir(string path)
        {
            path = path.Replace(" ", "_");
            short res = IO_CREATE_DIRECTORY.CreateDir(_s, Entries, path);
            return res;
        }

        private int clusterPos = 0;

        public short WriteFile(string path, byte[] fileData)
        {
            path = path.Replace(" ", "_");
            FatEntry? e = GetEntryFor(path);
            if (e != null)
            {
                if (e.Type == 'F') RemoveFile(e);
            }

            string[] parts = path.Split('/');
            parts[0] = "/";
            string eName = parts[parts.Length - 1];
            if (eName.Length > 47) throw new Exception("File name is too-long (max 47)");

            int[] entryIds = new int[parts.Length];

            int containerEntryId = 0;
            int existingEntryId = 0;
            for (var p = 0; p < parts.Length; p++)
            {
                FatEntry? pathEntry = (p == 0
                    ? _entries.FirstOrDefault(e => e.Name == parts[p])
                    : _entries.FirstOrDefault(e => e.Name == parts[p] && e.ParentId == entryIds[p - 1]));

                if (pathEntry == null)
                {
                    if (p == parts.Length - 2)
                        throw new Exception($"Part '/{parts[p]}' of directory not found for {path}");
                }
                else
                {
                    entryIds[p] = pathEntry.EntryId;

                    if (pathEntry.Type == 'D')
                        containerEntryId = pathEntry.EntryId;
                    if (pathEntry.Type == 'F')
                        existingEntryId = pathEntry.EntryId;
                }
            }

            List<FatCluster> allocClusters = new List<FatCluster>();

            if (existingEntryId > 0)
                allocClusters.AddRange(_clusters.Where(c => c.ENTRY == existingEntryId).OrderBy(c => c.ORDER));

            var allocSize = allocClusters.Sum(a => a.SIZE);
            if (allocSize < fileData.Length)
            {
                int zeroCycles = 0;
                for (int i = 0; i < _clusters.Count; i++)
                {
                    FatCluster c = _clusters[clusterPos];

                    clusterPos += 1;
                    if (clusterPos == _clusters.Count)
                    {
                        clusterPos = 0;
                        zeroCycles += 1;
                    }

                    if (zeroCycles == 2)
                    {
                        // if not alloc suficient space, exception

                        break;
                    }

                    if (c.ENTRY == 0)
                    {
                        allocClusters.Add(c);
                        if (c.ENTRY > 0)
                            allocClusters = allocClusters.OrderBy(a => a.ORDER).ToList();
                    }

                    if (allocClusters.Sum(a => a.SIZE - 12) > fileData.Length)
                        break;
                }
            }
            else
            {
                // free exceed clusters
                int clustersToKeep = 0;
                long accLen = 0;

                for (int c = 0; c < allocClusters.Count; c++)
                {
                    accLen += allocClusters[c].SIZE;
                    clustersToKeep += 1;

                    if (accLen >= fileData.Length)
                        break;
                }

                for (int c = 0; c < (allocClusters.Count - clustersToKeep); c++)
                {
                    FatCluster clusterToRem = allocClusters[clustersToKeep + c];

                    _s.Position = clusterToRem.START;
                    _s.Write(new byte[ClusterSize]);

                    clusterToRem.ORDER = 0;
                    clusterToRem.ENTRY = 0;

                    allocClusters.Remove(clusterToRem);
                }
            }


            FatEntry? fileEntry = (existingEntryId == 0
                ? _entries.FirstOrDefault(e => e.EntryId == 0)
                : _entries.FirstOrDefault(e => e.EntryId == existingEntryId));

            int entryId = (existingEntryId == 0
                ? _entries.Max(e => e.EntryId) + 1
                : existingEntryId);

            if (existingEntryId == 0)
            {
                byte[] b = new byte[64];
                using (MemoryStream ms = new MemoryStream(b))
                {
                    ms.Write("F"u8); // type 0
                    ms.Write(BitConverter.GetBytes(containerEntryId)); // parent id 1-4
                    ms.Write(BitConverter.GetBytes(entryId)); // entry id 5-8
                    ms.Write(BitConverter.GetBytes(DateTime.Now.Ticks)); // last-change 9-16
                    ms.Write(Encoding.ASCII.GetBytes(eName)); // name 17-32
                    fileEntry?.Update('F', containerEntryId, entryId, eName);

                    _s.Position = fileEntry.EntryPos;
                    _s.Write(b);
                    _s.Flush();
                }
            }

            byte[] cluster = new byte[ClusterSize];

            using (MemoryStream clst_ms = new MemoryStream(cluster))
            {
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    for (int i = 0; i < allocClusters.Count; i++)
                    {
                        FatCluster c = allocClusters[i];
                        clst_ms.Position = 0;

                        int regularSize = ClusterSize - 12;
                        long blockSize = (ms.Length - ms.Position > regularSize
                            ? regularSize
                            : ms.Length - ms.Position);

                        byte[] fData = new byte[blockSize];
                        ms.Read(fData);

                        int len = fData.Length;

                        clst_ms.Write(BitConverter.GetBytes(entryId)); // entry owned id
                        clst_ms.Write(BitConverter.GetBytes(i + 1));   // order
                        clst_ms.Write(BitConverter.GetBytes(len)); //len
                        clst_ms.Write(fData);

                        c.ORDER = (i + 1);
                        c.ENTRY = entryId;
                        c.LEN = len;

                        _s.Position = c.START;
                        _s.Write(cluster);
                        _s.Flush();
                    }
                }

            }

            return 1;
        }



        // reads the file on disk from path
        public byte[] ReadFile(string path)
        {
            path = path.Replace(" ", "_");
            string[] parts = path.Split('/');
            parts[0] = "/";
            string eName = parts[parts.Length - 1];

            int[] entryIds = new int[parts.Length];

            int containerEntryId = 0;
            int existingEntryId = 0;
            for (var p = 0; p < parts.Length; p++)
            {
                FatEntry? pathEntry = (p == 0
                    ? _entries.FirstOrDefault(e => e.Name == parts[p])
                    : _entries.FirstOrDefault(e => e.Name == parts[p] && e.ParentId == entryIds[p - 1]));

                if (pathEntry == null)
                    throw new Exception($"Part '/{parts[p]}' of directory not found for {path}");
                else
                {
                    entryIds[p] = pathEntry.EntryId;

                    if (pathEntry.Type == 'D')
                        containerEntryId = pathEntry.EntryId;
                    if (pathEntry.Type == 'F')
                        existingEntryId = pathEntry.EntryId;
                }
            }

            List<FatCluster> allocClusters = new List<FatCluster>();

            if (existingEntryId > 0)
                allocClusters.AddRange(_clusters.Where(c => c.ENTRY == existingEntryId).OrderBy(c => c.ORDER));

            byte[] readBuffer = new byte[allocClusters.Sum(a => a.LEN)];
            using (MemoryStream msRead = new MemoryStream(readBuffer))
            {
                for (int c = 0; c < allocClusters.Count; c++)
                {
                    var cluster = allocClusters[c];

                    _s.Position = cluster.START + 12; // + 8 = skip cluster head

                    byte[] buffer = new byte[cluster.LEN];
                    _s.Read(buffer);

                    int readIndx = (int)allocClusters[c].LEN;// Array.IndexOf(buffer, (byte)'\0');
                    if (readIndx <= 0)
                        readIndx = buffer.Length;

                    msRead.Write(buffer, 0, readIndx);
                }
                /*
                msRead.Position = 0;
                int readedIndx = Array.IndexOf(readBuffer, (byte)'\0');
                if (readedIndx <= 0)
                    readedIndx = readBuffer.Length;
                */
                return readBuffer;
            }
        }

        public short RemoveDir(string path)
        {
            path = path.Replace(" ", "_");
            throw new NotImplementedException();
        }

        public short RemoveDir(FatEntry e)
        {
            if (e == null) return 0;
            if (e.Type != 'D') return 0;

            var childs = _entries.Where(x => x.ParentId == e.EntryId);
            foreach (var child in childs)
            {
                if (child.Type == 'F')
                {
                    RemoveFile(child);
                }
                else
                {
                    if (HasChilds(child)) RemoveDir(child);
                    else
                    {
                        _s.Position = child.EntryPos;
                        _s.Write(new byte[64]);

                        child.Update('\0', 0, 0, null);
                    }
                }
            }


            _s.Position = e.EntryPos;
            _s.Write(new byte[64]);
            _s.Flush();

            e.Update('\0', 0, 0, null);
            return 1;
        }


        public short RemoveFile(FatEntry? e)
        {
            if (e == null) return 0;
            if (e.Type != 'F') return 0;

            List<FatCluster> usedClusters = _clusters.Where(c => c.ENTRY == e.EntryId).ToList();

            foreach (var c in usedClusters)
            {
                _s.Position = c.START + 12;
                long size = (c.LEN - 12);
                byte[] empty = new byte[size];
                _s.Write(empty);

                _s.Position = c.START;
                _s.Write(BitConverter.GetBytes((int)0)); // entry owner id
                _s.Write(BitConverter.GetBytes((int)0)); // order
                _s.Write(BitConverter.GetBytes((int)0)); // size

                c.Free();
            }

            _s.Flush();

            _s.Position = e.EntryPos;
            e.Update('\0', 0, 0, null);
            _s.Position = e.EntryPos;
            _s.Write(new byte[64]);

            _s.Flush();
            return 1;
        }


        public short RemoveFile(string path)
        {
            FatEntry? e = GetEntryFor(path);
            return RemoveFile(e);
        }


        public IReadOnlyCollection<FileDirectory> ListDir(string path)
        {
            path = path.Replace(" ", "_");
            List<FileDirectory> res = new List<FileDirectory>();

            string[] parts = path.Split('/');
            parts[0] = "/";
            string eName = parts[parts.Length - 1];
            if (eName.Length > 47) throw new Exception("File name is too-long (max 15)");
            int[] entryIds = new int[parts.Length];
            int targetDirEntry = 0;
            for (var p = 0; p < parts.Length; p++)
            {
                FatEntry? pathEntry = (p == 0
                    ? _entries.FirstOrDefault(e => e.Name == parts[p])
                    : _entries.FirstOrDefault(e => e.Name == parts[p] && e.ParentId == entryIds[p - 1]));

                if (parts[p] == "") continue;
                if (pathEntry == null)
                {
                    if (p == parts.Length - 2)
                        return null;
                }
                else
                {
                    entryIds[p] = pathEntry.EntryId;

                    if (pathEntry.Type == 'D')
                    {
                        targetDirEntry = pathEntry.EntryId;

                    }

                }
            }

            var subs = _entries.Where(e => e.ParentId == targetDirEntry).ToList();

            foreach (var sub in subs)
            {
                if (sub.Type == 'D')
                {
                    long len = _entries.Where(e => !string.IsNullOrEmpty(e.Name))
                      .Where(e => e.ParentId == sub.EntryId)
                      .Sum(e =>
                          _clusters.Where(c => c.ENTRY == e.EntryId).Sum(c => c.LEN)
                       );
                    res.Add(new FileDirectory
                    {
                        Name = sub.Name,
                        Type = "directory",
                        Size = $"{Ext.HumanHeadableBytesString(len)}"
                    });
                }
                if (sub.Type == 'F')
                    res.Add(new FileDirectory
                    {
                        Name = sub.Name,
                        Type = sub.Name.EndsWith(".o") ? "program" : "file",
                        Size = $"{Ext.HumanHeadableBytesString(_clusters.Where(c => c.ENTRY == sub.EntryId).Sum(c => c.LEN))}"
                    });
            }

            return res.OrderBy(f => f.Type).ToList();
        }

        public bool DirectoryExists(string dir)
        {
            try
            {
                string name = dir;

                if (string.IsNullOrEmpty(name)) name = "/";

                string[] dirParts = dir.Split('/');

                int start = (dirParts[0] == "" ? 1 : 0);
                FatEntry? last_e = _entries.FirstOrDefault(e => e.Name == "/" && e.Type == 'D');
                if (name == "/") return true;
                for (int i = start; i < dirParts.Length; i++)
                {
                    string part = dirParts[i];
                    FatEntry? ex = _entries.FirstOrDefault(e => e.Name == part && e.Type == 'D' && e.ParentId == last_e.EntryId);
                    if (ex == null) return false;

                    last_e = ex;
                }

                return last_e.Type == 'D';
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Fatal disk error: maybe this disk was damage; run ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[check-disk] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("to detect and repair corrupted clusters or head-entries");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Exception details: \n{ex.Message}");
                return false;
            }
        }


        public FatEntry? GetParent(FatEntry e)
        {
            FatEntry? parent = _entries.FirstOrDefault(x => x.EntryId == e.ParentId);
            return parent;
        }

        public FatEntry? GetEntryFor(string dir)
        {
            dir = dir.Replace(" ", "_");
            string name = dir;

            if (string.IsNullOrEmpty(name)) name = "/";

            string[] dirParts = dir.Split('/');

            FatEntry? last_e = _entries.FirstOrDefault(e => e.Name == "/" && e.Type == 'D');
            for (int i = 1; i < dirParts.Length; i++)
            {
                string part = dirParts[i];
                FatEntry? ex = _entries.FirstOrDefault(e => e.Name == part && e.ParentId == last_e.EntryId);
                if (ex == null) return null;

                last_e = ex;
            }

            return last_e;
        }

        public bool HasChilds(FatEntry e)
        {
            bool hasChilds = _entries.Any(x => x.ParentId == e.EntryId);
            return hasChilds;
        }

        public short ChangeParent(FatEntry child, FatEntry newParent)
        {
            child.SetParent(newParent);

            _s.Position = child.EntryPos + 1;
            _s.Write(BitConverter.GetBytes(newParent.EntryId));
            _s.Flush();

            return 1;
        }
    }
}
