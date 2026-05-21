
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mShort: mOSObject
    {
        [FieldOrder(0)]
        public short Value { get; set; }

        public mShort()
        {

        }

        public mShort(short value)
        {
            this.Value = value;
        }
    }
}
