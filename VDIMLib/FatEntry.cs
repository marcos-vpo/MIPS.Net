using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib
{
    public class FatEntry
    {
        public long EntryPos { get; private set; }
        public char Type { get; private set; }
        public int ParentId { get; private set; }
        public int EntryId { get; private set; }
        public DateTime? LastChange { get; private set; }
        public string? Name { get; private set; }

        public override string ToString()
        {
            if (EntryId == 0) return $"(0x{EntryPos}) unused entry";
            return $"(0x{EntryPos}) [{Type}] {Name}";
        }

        public FatEntry(long pos, char type, int parentId, int entryId, DateTime? creation, string name)
        {
            EntryPos = pos;
            Type = type;
            ParentId = parentId;
            EntryId = entryId;
            LastChange = creation;

            if (name.Contains('\0'))
                Name = name.Substring(0, name.IndexOf('\0'));
            else
                Name = name;
        }

        public FatEntry()
        {
            Type = '\0';
            ParentId = 0;
            EntryId = 0;
            LastChange = null;
            Name = null;
        }

        public void Update(char type, int parentId, int entryId, string name)
        {
            Type = type;
            ParentId = parentId;
            EntryId = entryId;
            LastChange = DateTime.Now;
            Name = name;
        }

        internal void SetParent(FatEntry newParent)
        {
            ParentId = newParent.EntryId;
        }
    }
}
