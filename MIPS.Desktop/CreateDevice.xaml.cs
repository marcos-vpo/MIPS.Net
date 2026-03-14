using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using MIPS.Desktop.Utils;
using MIPS.Desktop.ViewModel;

namespace MIPS.Desktop
{
    /// <summary>
    /// Lógica interna para CreateDevice.xaml
    /// </summary>
    public partial class CreateDevice : Window
    {
        public CreateDevice()
        {
            InitializeComponent();
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
            imgDevice.Source = bitmap;
        }

        private void rdoStorage_Checked(object sender, RoutedEventArgs e)
        {
            SetDev("hd");
            lbInfoDev.Text = "Transfer files between emulator and a external program";
        }

        private void rdoDisplay_Checked(object sender, RoutedEventArgs e)
        {
            SetDev("screen");
            lbInfoDev.Text = "Display in Text-Mode and Graphical Mode in a external program";
        }

        private void rdoPrinter_Checked(object sender, RoutedEventArgs e)
        {
            SetDev("printer");
            lbInfoDev.Text = "Real or Simulated printer device controlled by a external program";
        }

        private void rdoNet_Checked(object sender, RoutedEventArgs e)
        {
            SetDev("network");
            lbInfoDev.Text = "A Network emulated external program that controlling 1 TCP/IP port and 1 HTTP port";
        }

        private void rdoDebug_Checked(object sender, RoutedEventArgs e)
        {
            SetDev("remote_deb");
            lbInfoDev.Text = "An Remote Debugger special program that allows inspect System's execution";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeviceVM dev = new DeviceVM();
                dev.Name = txName.Text;
                dev.ExePath = txExe.Text;

                if (rdoStorage.IsChecked.Value) dev.Type = "storage";
                if (rdoDisplay.IsChecked.Value) dev.Type = "display";
                if (rdoPrinter.IsChecked.Value) dev.Type = "printer";
                if (rdoNet.IsChecked.Value) dev.Type = "network";
                if (rdoDebug.IsChecked.Value) dev.Type = "debug";

                Devices.CreateDevice(dev);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MIPS.Net", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnSelExe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Program File|*.exe";
            ofd.ShowDialog();

            if (!File.Exists(ofd.FileName)) return;

            txExe.Text = ofd.FileName;
        }
    }
}
