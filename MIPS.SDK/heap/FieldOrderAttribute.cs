using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOSLib.heap
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class FieldOrderAttribute : Attribute
    {
        public FieldOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

}
