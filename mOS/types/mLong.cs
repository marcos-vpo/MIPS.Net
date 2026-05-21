
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mLong : mOSObject
    {
        [FieldOrder(0)]
        public long Value { get; set; }

        public mLong()
        {

        }

        public mLong(long value)
        {
            this.Value = value;
        }
    }
}
