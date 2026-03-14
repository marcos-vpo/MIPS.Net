namespace MIPS.Net.Debugger
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
}
