namespace MIPS.Net.Debugger
{
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
        public int RotuleN { get; set; }
        public string Name { get; set; }
        public int Ln { get; set; }
        public int StartAddr { get; set; }
        public int EndAddr { get; set; }

        internal void AddInstruction(RotuleInstruction rotuleInstruction)
        {
            Instructions.Add(rotuleInstruction);
        }
    }
}
