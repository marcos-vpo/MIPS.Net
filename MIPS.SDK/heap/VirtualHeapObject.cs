 

namespace mOSLib.heap
{
    internal class VirtualHeapObject
    {
        public int VirtualHeapAddr { get; set; }
        // [ 2 + 2 + 4 + 4 + N... ] = 
        public short Type { get; set; }
        public short Flags { get; set; }
        public int ContinuationAddr { get; set; }
        public int HObjSize { get; set; }

    }
}
