
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mByteArray : mOSObject
    {
        [FieldOrder(0)]
        public byte[] Value { get; set; }

        public mByteArray()
        {

        }

        public mByteArray(byte[] value)
        {
            this.Value = value;
        } 
    }
}
