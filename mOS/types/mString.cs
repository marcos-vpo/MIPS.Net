
using mOS.k_objects;
using mOS.misc;

namespace mOS.types
{
    internal class mString : mOSObject
    {
        [FieldOrder(0)]
        public string Value { get; set; }

        public mString()
        {
            this.Set(this);
        }

        public mString(string value)
        {
            this.Value = value;
            this.Set(this);
        }

        public override string ToString()
        {
            return Value;
        }

    }
}
