using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.InstructionSet;
using MIPS.Net.SoC;

namespace MIPS.Net.abi
{
    internal class InstructionParser
    {
        public static byte[] __parse(string str, Registers registers)
        {
            string[] parts = str.Trim().Split(' ');
            if (parts[0] == "div") return div.__parse(str, registers);
            if (parts[0] == "mflo") return mflo.__parse(str, registers);
            if (parts[0] == "mfhi") return mfhi.__parse(str, registers);
            if (parts[0] == "addi") return addi.__parse(str, registers);
            if (parts[0] == "addiu") return addiu.__parse(str, registers);
            if (parts[0] == "mul") return mul.__parse(str, registers);
            if (parts[0] == "sub") return sub.__parse(str, registers);
            if (parts[0] == "andi") return andi.__parse(str, registers);
            if (parts[0] == ("lb")) return lb.__parse(str, registers);
            if (parts[0] == ("sb")) return sb.__parse(str, registers);
            if (parts[0] == ("syscall")) return syscall.__parse(str, registers);
            if (parts[0] == ("lar")) return lar.__parse(str, registers);
            if (parts[0] == ("la")) return la.__parse(str, registers);
            if (parts[0] == ("lw")) return lw.__parse(str, registers);
            if (parts[0] == ("sw")) return sw.__parse(str, registers);
            if (parts[0] == ("li")) return li.__parse(str, registers);
            if (parts[0] == "lui") return lui.__parse(str, registers);
            if (parts[0] == "ori") return ori.__parse(str, registers);
            if (parts[0] == "jr") return jr.__parse(str, registers);
            if (parts[0] == "jal") return jal.__parse(str, registers);
            if (parts[0] == "jalf") return jalf.__parse(str, registers);
            if (parts[0] == "add") return add.__parse(str, registers);
            if (parts[0] == "beq") return beq.__parse(str, registers);

            if (parts[0] =="slt") return slt.__parse(str, registers);
            if (parts[0] == "sltu") return sltu.__parse(str, registers);
            if (parts[0] == "sgt") return sgt.__parse(str, registers);
            if (parts[0] == "sgtu") return sgtu.__parse(str, registers);
            if (parts[0] == "seq") return seq.__parse(str, registers);
            if (parts[0] == "sne") return sne.__parse(str, registers);
            if (parts[0] == "sge") return sge.__parse(str, registers);
            if (parts[0] == "sgeu") return sgeu.__parse(str, registers);
            if (parts[0] == "move") return move.__parse(str, registers);
            return new byte[0];
        }
    }
}
