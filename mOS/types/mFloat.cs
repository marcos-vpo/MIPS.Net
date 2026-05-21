
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mFloat : mOSObject
    {
        [FieldOrder(0)]
        public float Value { get; set; }

        public mFloat()
        {

        }

        public mFloat(float value)
        {
            this.Value = value;
        }
    }
}
