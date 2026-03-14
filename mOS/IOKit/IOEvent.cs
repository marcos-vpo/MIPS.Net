using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.IOKit
{
    internal enum IOEvent
    {
        DATA_RECEIVED = 0,
        DEVICE_CONNECTED = 1,
        DEVICE_DISCONNECTED = 2,
        DATA_SEND_FAILED = 3,
        DATA_RECEIVE_FAILED = 4
    }
}
