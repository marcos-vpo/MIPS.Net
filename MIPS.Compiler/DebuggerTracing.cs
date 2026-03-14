using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Compiler
{
    public class TracingDataEntry
    {
        public TracingDataEntry(int index, string declaration, int ln)
        {
            Index = index;
            Declaration = declaration.Trim();
            Ln = ln;
        }

        public TracingDataEntry()
        {

        }

        public int Index { get; }
        public string Declaration { get; }
        public int Ln { get; }
    }

    public class TracingRotule
    {
        public TracingRotule(int rotuleN, string name, int ln)
        {
            RotuleN = rotuleN;
            Name = name;
            Ln = ln;
            Instructions = new List<RotuleInstruction>();
        }

        public List<RotuleInstruction> Instructions { get; set; }

        public TracingRotule()
        {
            Instructions = new List<RotuleInstruction>();
        }
        public int RotuleN { get; }
        public string Name { get; }
        public int Ln { get; }

        internal void AddInstruction(RotuleInstruction rotuleInstruction)
        {
            Instructions.Add(rotuleInstruction);
        }
    }

    public class RotuleInstruction
    {
        public RotuleInstruction(int code, string definition, string hex, int ln)
        {
            Code = code;
            Definition = definition;
            Hex = hex;
            Ln = ln;
        }

        public int MemAddr { get; set; }
        public int Code { get; }
        public string Definition { get; }
        public string Hex { get; }
        public int Ln { get; }
    }

    public class DebuggerTracing
    {
        public List<TracingDataEntry> DataEntries { get; set; }
        public List<TracingRotule> Rotules { get; set; }
        public string SourceFile { get; internal set; }

        public DebuggerTracing()
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
    }
}
