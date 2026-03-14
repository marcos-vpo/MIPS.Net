using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIPS.Net.IO;
using NetPC.Controllers;
using NetPC.Properties;

namespace NetPC
{
    public partial class UIPort : UserControl
    {
        private readonly USBPort port;
        private readonly IDeviceMediator deviceMediator;

        public UIPort(USBPort port, IDeviceMediator deviceMediator)
        {
            InitializeComponent();

            port.DeviceTypeDetected += Port_DeviceTypeDetected;
            port.OnOffChanged += Port_OnOffChanged;
            port.ConnectionChanged += Port_ConnectionChanged;
            this.port = port;
            this.deviceMediator = deviceMediator;
        }

        private void Port_ConnectionChanged(bool connected)
        {
            img.BackgroundImage = (connected ? Resources.usb_on : Resources.usb_idle);
        }

        private void Port_OnOffChanged(bool is_on)
        {
            if (is_on)
            {

                Task.Run(() => Thread.Sleep(500)).ContinueWith((t) =>
                {
                    Invoke(() =>
                    {
                        try
                        {

                            if (port.Class == DeviceClass.DISPLAY) deviceMediator.TurnOnScreen(port);
                            if (port.Class == DeviceClass.STORAGE) deviceMediator.TurnOnStorage(port);
                            if (port.Class == DeviceClass.KEYBOARD) deviceMediator.TurnOnKeyBoard(port);

                        }
                        catch (Exception ex)
                        {
                            img.BackgroundImage = Resources.usb_idle;
                        }
                    });
                });
            }
            else
            {
                Invoke(() =>
                {
                    img.BackgroundImageLayout = ImageLayout.Center;
                    img.BackgroundImage = Resources.usb_off;
                    if (port.Class == DeviceClass.DISPLAY) deviceMediator.TurnOffScreen(port);
                    if (port.Class == DeviceClass.STORAGE) deviceMediator.TurnOffStorage(port);
                    if (port.Class == DeviceClass.KEYBOARD) deviceMediator.TurnOffKeyBoard(port);
                });
            }
        }

        private string deviceType = "";
        private void Port_DeviceTypeDetected(string dev_type)
        {
            img.BackgroundImageLayout = ImageLayout.Stretch;
            this.deviceType = dev_type;
            if (dev_type == "storage") img.BackgroundImage = Resources.hd; //disp.Invoke(() => SetDev("hd"));
            else if (dev_type == "printer") img.BackgroundImage = Resources.printer;
            else if (dev_type == "display") img.BackgroundImage = Resources.screen;
            else if (dev_type == "mouse") img.BackgroundImage = Resources.mouse;
            else if (dev_type == "keyboard") img.BackgroundImage = Resources.keyboard;
            else if (dev_type == "network") img.BackgroundImage = Resources.network;
            else
            {
                img.BackgroundImageLayout = ImageLayout.Center;
                img.BackgroundImage = Resources.usb_on;
            }
        }
    }
}
