using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.IO;

namespace NetPC
{
    public interface IDeviceMediator
    {
        void TurnOnStorage(USBPort port);
        void TurnOffStorage(USBPort port);

        void TurnOnScreen(USBPort port);
        void TurnOffScreen(USBPort port);

        void TurnOnKeyBoard(USBPort port);
        void TurnOffKeyBoard(USBPort port);

    }
}
