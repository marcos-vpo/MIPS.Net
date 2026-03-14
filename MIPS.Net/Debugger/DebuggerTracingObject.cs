using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.Debugger
{
    public class DebuggerTracingObject
    {
        public List<TracingDataEntry> DataEntries { get; set; }
        public List<TracingRotule> Rotules { get; set; }
        public string SourceFile { get; set; }

        public DebuggerTracingObject()
        {
            DataEntries = new List<TracingDataEntry>();
            Rotules = new List<TracingRotule>();
        }

        internal void AddDataEntry(int index, string declaration, int ln)
        {
            DataEntries.Add(new TracingDataEntry(index, declaration, ln));
        }

        internal void AddInstruction(int code, string definition, string hex, int ln)
        {
            TracingRotule last = Rotules.Last();
            last.AddInstruction(new RotuleInstruction(code, definition, hex, ln));
        }

        internal void AddRotule(int rotuleN, string name, int ln)
        {
            Rotules.Add(new TracingRotule(rotuleN, name, ln));
        }

        internal TracingRotule? GetRotule(int rotule)
        {
            return Rotules.FirstOrDefault(r => r.RotuleN == rotule);
        }
    }
}
