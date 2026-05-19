 
namespace mOSLib.heap
{
    internal class ProcessHeapPage
    {
        public ProcessHeapPage( 
            int pId,
            int order, 
            int page,
            int usage)
        { 
          
            ProcessId = pId;    
            Order = order;
            PageIndex = page;
            Usage = usage;
        } 

        public int ProcessId { get; private set; }
        public int Order { get; private set; }
        public int PageIndex { get; private set; }
        public int Usage { get; private set; }
    }
}
