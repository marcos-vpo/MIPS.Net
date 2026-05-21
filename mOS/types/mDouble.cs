
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mDouble : mOSObject
    {
        [FieldOrder(0)]
        public double Value { get; set; }

        public mDouble()
        {

        }

        public mDouble(double value)
        {
            this.Value = value;
        }
    }
}
