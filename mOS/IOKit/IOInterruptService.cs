using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using MIPS.Net.abi;

namespace mOS.IOKit
{
    public class IOInterruptService : ABIManaged
    {
        private static IOInterruptService _instance;

        public IOInterruptService()
        {
            _instance = this;
        }

        /// <summary>
        /// Occurs on data was received from an external device
        /// </summary>
        /// <param name="a0">interruption code</param>
        /// <param name="a1">device mem address</param>
        [Extern]
        public void DeviceDataReceived(int a0, int a1)
        {
            int intr_code = a0;
            int device_addr = a1;
            IOService svc = IORegistry.GetService(device_addr);
            if (svc == null) return;

            svc.DataReceived();
        }


        /// <summary>
        /// Occurs on an external device was connected 
        /// </summary>
        /// <param name="a0">interruption code</param>
        /// <param name="a1">device mem address</param>
        [Extern]
        public void DeviceConnected(int a0, int a1)
        {
            int intr_code = a0;
            int device_addr = a1;
            IOService svc = IORegistry.GetService(device_addr);
            if (svc == null) return;

            svc.DeviceConnected();
        }

        /// <summary>
        /// Occurs on an external device was disconnected 
        /// </summary>
        /// <param name="a0">interruption code</param>
        /// <param name="a1">device mem address</param>
        [Extern]
        public void DeviceDisconnected(int a0, int a1)
        {
            int intr_code = a0;
            int device_addr = a1;
            IOService svc = IORegistry.GetService(device_addr);
            if (svc == null) return;

            svc.DeviceDisconnected();
        }


        /// <summary>
        /// Occurs on an external device was data send failed 
        /// </summary>
        /// <param name="a0">interruption code</param>
        /// <param name="a1">device mem address</param>
        [Extern]
        public void DeviceSendFail(int a0, int a1)
        {
            int intr_code = a0;
            int device_addr = a1;
            IOService svc = IORegistry.GetService(device_addr);
            if (svc == null) return;

            svc.SendDataFailed();
        }

        /// <summary>
        /// Occurs on an external device was data receive failed 
        /// </summary>
        /// <param name="a0">interruption code</param>
        /// <param name="a1">device mem address</param>

        [Extern]
        public void DeviceReceiveFail(int a0, int a1)
        {
            int intr_code = a0;
            int device_addr = a1;
            IOService svc = IORegistry.GetService(device_addr);
            if (svc == null) return;

            svc.ReceiveDataFailed();
        }
    }
}
