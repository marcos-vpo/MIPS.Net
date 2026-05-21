 
namespace mOSLib.heap
{
    internal class VirtualHeapPage
    {
        public VirtualHeapPage( 
            int processId,
            int virtualOrder, 
            int virtualPageIndex,
            int physicalPage,
            int usage)
        { 
          
            ProcessId = processId;    
            Order = virtualOrder;
            VirtualPage = virtualPageIndex;
            PhysicalPage = physicalPage;    
            Usage = usage;
        } 

        public int ProcessId { get; private set; }
        public int Order { get; private set; }
        public int VirtualPage { get; private set; }
        public int PhysicalPage { get; private set; }
        public int Usage { get; private set; }
    }
}
