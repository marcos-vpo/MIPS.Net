
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mByte
    {
        [FieldOrder(0)]
        public byte Value { get; set; }

        public mByte()
        {

        }

        public mByte(byte value)
        {
            this.Value = value;
        }
    }
}
