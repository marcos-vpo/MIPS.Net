using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MIPS.Desktop.Utils;
using MIPS.Desktop.ViewModel;

namespace MIPS.Desktop
{
    /// <summary>
    /// Lógica interna para CreateSystem.xaml
    /// </summary>
    public partial class CreateSystem : Window
    {
        public CreateSystem(SystemVM system = null)
        {
            InitializeComponent();

            List<KeyValuePair<byte, string>> ports = new List<KeyValuePair<byte, string>>();
            ports.Add(new KeyValuePair<byte, string>((byte)0x20, "USB (class 0x20)"));
            ports.Add(new KeyValuePair<byte, string>((byte)0x30, "SERIAL (class 0x30)"));
            ports.Add(new KeyValuePair<byte, string>((byte)0x40, "VGA (class 0x40)"));

            cbPortType.ItemsSource = ports;
            cbPortType.DisplayMemberPath = "Value";
            cbPortType.SelectedValuePath = "Key";


            cbDevice.ItemsSource = Devices.GetDevices();
            cbDevice.DisplayMemberPath = "Name";
            cbDevice.SelectedValuePath = "Name";

            if(system !=null)
            {
                mySystem = system;
                txName.Text = system.Name;
                txVendor.Text = system.Vendor;
                txMem.Text = system.MemorySize.ToString();
                dataGridPorts.ItemsSource = mySystem.Ports;
                dataGridButtons.ItemsSource = mySystem.Buttons;
            }

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mySystem.Name = txName.Text;
                mySystem.Vendor = txVendor.Text;
                mySystem.MemorySize = int.Parse(txMem.Text);

                Systems.CreateSystem(mySystem);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MIPS.Net", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private SystemVM mySystem = new SystemVM();
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SystemPort port = new SystemPort();
            port.Class = (byte)cbPortType.SelectedValue;
            port.ID = (byte)(mySystem.Ports == null ? 1 : mySystem.Ports.Count + 1);
            port.Device = (DeviceVM)cbDevice.SelectedItem;

            mySystem.Ports.Add(port);
            dataGridPorts.ItemsSource = mySystem.Ports;
            dataGridPorts.Items.Refresh();
        }

        private void btnNewDev_Click(object sender, RoutedEventArgs e)
        {
            new CreateDevice().ShowDialog();
            cbDevice.ItemsSource = Devices.GetDevices();
            cbDevice.DisplayMemberPath = "Name";
            cbDevice.SelectedValuePath = "Name";
        }

        private void btnAddSysButton_Click(object sender, RoutedEventArgs e)
        {
            SystemButton btn = new SystemButton();
            btn.ID = (byte)(80 + mySystem.Buttons.Count + 1);
            btn.Text = txButtoNText.Text;

            mySystem.Buttons.Add(btn);
            dataGridButtons.ItemsSource = mySystem.Buttons;
            dataGridButtons.Items.Refresh();

            txButtoNText.Text = "";
            txButtoNText.Focus();
        }
    }
}
