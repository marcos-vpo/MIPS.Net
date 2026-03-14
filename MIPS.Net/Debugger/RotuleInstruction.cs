namespace MIPS.Net.Debugger
{
    public class RotuleInstruction
    {
        public RotuleInstruction(int code, string definition, string hex, int ln)
        {
            Code = code;
            Definition = definition;
            Hex = hex;
            Ln = ln;
        }

        public RotuleInstruction()
        {
                
        }

        public int MemAddr { get; set; }
        public int Code { get; set; }
        public string Definition { get; set; }
        public string Hex { get; set; }
        public int Ln { get; set; }
    }
}
