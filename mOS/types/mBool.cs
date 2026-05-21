 
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mBool : mOSObject
    {
        [FieldOrder(0)]
        public long Value { get; set; }

        public mBool()
        {

        }

        public mBool(long value)
        {
            this.Value = value;
        }
    }
}
