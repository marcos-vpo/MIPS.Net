using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MIPS.Desktop.ViewModel;
using MIPS.Net.IO;
using MIPS.Net.SoC;

namespace MIPS.Desktop
{
    /// <summary>
    /// Interação lógica para UIPort.xam
    /// </summary>
    public partial class UIPort : UserControl
    {
        private readonly IOPort _io_port;

        public UIPort(IOPort io_port)
        {
            InitializeComponent();
            _io_port = io_port;

            if (io_port is USBPort)
            {
                (_io_port as USBPort).ConnectionChanged += _io_port_ConnectionChanged;
                (_io_port as USBPort).DeviceTypeDetected += _io_port_DeviceTypeDetected;
                (_io_port as USBPort).OnOffChanged += _io_port_OnOffChanged;
            }
            if (io_port.Tag != null)
            {
                if (io_port.Tag is SystemPort)
                {
                    var sp = (SystemPort)io_port.Tag;
                    lbDev.Text = sp.ClassStr + $"\nHWID: {sp.IDStr}";
                }
            }
        }

        Process proc = null;
        private void _io_port_OnOffChanged(bool is_on)
        {
            if (is_on)
            {
                SystemPort port = (SystemPort)_io_port.Tag;
                if (port.Device != null)
                {
                    if (File.Exists(port.Device.ExePath))
                    {
                        if (File.ReadAllText("AUTO_PORTS_ON.txt") == "1")
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = port.Device.ExePath;
                            psi.Arguments = _io_port.DevicePort.ToString();
                            psi.WorkingDirectory = new FileInfo(psi.FileName).DirectoryName;
                            //    psi.UseShellExecute = true;

                            proc = new Process();
                            proc.StartInfo = psi;
                            proc.Start();
                            Thread.Sleep(200);
                        }
                    }
                }
            }
            else
            {
                var sp = (SystemPort)_io_port.Tag;
                Application.Current.Dispatcher.Invoke(() => lbDev.Text = sp.ClassStr + $"\nHWID: {sp.IDStr}");
                try { if (proc != null) Process.GetProcessById(proc.Id).Kill(); } catch { }
            }


            Application.Current.Dispatcher.Invoke(() => elStatus.Stroke = is_on ? Brushes.Red : Brushes.Black);
        }

        private void _io_port_DeviceTypeDetected(string dev_type)
        {
            var disp = Application.Current.Dispatcher;
            disp.Invoke(() => lbDev.Text += $"\n[{dev_type.ToUpper()}]");
            if (dev_type == "storage") disp.Invoke(() => SetDev("hd"));
            if (dev_type == "printer") disp.Invoke(() => SetDev("printer"));
            if (dev_type == "display") disp.Invoke(() => SetDev("screen"));
            if (dev_type == "mouse") disp.Invoke(() => SetDev("mouse"));
            if (dev_type == "keyboard") disp.Invoke(() => SetDev("keyboard"));
        }

        private void _io_port_ConnectionChanged(bool connected)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var sp = (SystemPort)_io_port.Tag;
                Application.Current.Dispatcher.Invoke(() => lbDev.Text = sp.ClassStr + $"\nHWID: {sp.IDStr}");

                elStatus.Stroke = connected ? Brushes.Green : Brushes.Red;
                if (!connected)
                    imgDev.Source = null;
            });
        }

        private void SetDev(string dev)
        {
            // Cria um novo objeto BitmapImage
            BitmapImage bitmap = new BitmapImage();

            // Inicializa o objeto BitmapImage
            bitmap.BeginInit();
            // Define o URI da imagem. Pode ser um caminho relativo ou absoluto.
            bitmap.UriSource = new Uri($@".\icons\{dev}.png", UriKind.Relative);
            bitmap.EndInit();

            // Define a propriedade Source do controle Image
            imgDev.Source = bitmap;
       
        }
    }
}
