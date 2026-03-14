using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vdim
{
    public abstract class VCommand
    {
        public abstract void Run(string[] args);

        protected string ReadString(string[] args, int index, int Range, string msg)
        {
            if ((args.Length - 1) < index)
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    Console.Write(msg);
                    string res = Console.ReadLine();
                    if (string.IsNullOrEmpty(res)) Environment.Exit(0);

                    return res;
                }
                return "";
            }
            else
            {
                string value = "";
                for (int i = index; i < (index + Range); i++)
                    value += args[i];
                return value;
            }
        }


        protected int ReadInt(string[] args, int index, string msg)
        {
            int resInt = 0;
            if ((args.Length - 1) < index)
            {
                Console.Write(msg);
                string res = Console.ReadLine();
                if (string.IsNullOrEmpty(res)) Environment.Exit(0);


                int.TryParse(res, out resInt);
                if (resInt == 0) Environment.Exit(0);
                return resInt;
            }
            else
            {
                int.TryParse(args[index], out resInt);
                if (resInt == 0) Environment.Exit(0);
                return resInt;
            }
        }
    }
}
