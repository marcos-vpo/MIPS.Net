
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mInt : mOSObject
    {
        [FieldOrder(0)]
        public int Value { get; set; }

        public mInt()
        {

        }

        public mInt(int value)
        {
            this.Value = value;
        }
    }
}
