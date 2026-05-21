 
namespace mOSLib.heap
{
    internal class VirtualHeapFragment
    {

        public VirtualHeapFragment(int virtualHeapAddr, int length)
        {
            VirtualHeapStart = virtualHeapAddr;
            Length = length;
        }

        public override string ToString()
        {
            return $"[*{VirtualHeapStart}, {Length}b";
        }

        public int VirtualHeapStart { get; set; }
        public int Length { get; set; }
    }
}
