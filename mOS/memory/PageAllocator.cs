using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace mOS.memory
{
    internal class PageAllocator
    {
        private readonly PhysicalPageManager physical_page_manager;
        public PageAllocator(PhysicalPageManager ppm)
        {
            physical_page_manager = ppm;
        }

        public void Allocate(int bytes, out int[] physPages)
        {
            MemInfo info = physical_page_manager.GetInfo();

            if (info.Free > bytes)
                physPages = physical_page_manager.AllocFromFree(bytes);
            else
                physPages = physical_page_manager.AllocFromInactive(bytes, inactiveTimeMinutes: 2);
        }

        public void ProtectedAllocate(int bytes, out int[] physPages)
        {

            MemInfo info = physical_page_manager.GetInfo();
            var x = info.ToString();
            if (info.Free > bytes)
                physPages = physical_page_manager.AllocFromFree(bytes, wired: true);
            else
                physPages = physical_page_manager.AllocFromInactive(bytes, inactiveTimeMinutes: 2, wired: true);
        }

        internal void MoveToInactive(int pageIndex)
        {
            physical_page_manager.MoveToInactive(pageIndex);
        }

        internal void MoveToFree(int pageIndex)
        {
            physical_page_manager.MoveToFree(pageIndex);
        }

        internal void UpdateLastAccess(int pageIndex)
        {
            physical_page_manager.UpdateLastAccess( pageIndex);
        }
    }
}
