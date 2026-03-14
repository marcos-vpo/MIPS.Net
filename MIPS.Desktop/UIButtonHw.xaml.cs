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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MIPS.Net.IO;
using MIPS.Net.SoC;

namespace MIPS.Desktop
{
    /// <summary>
    /// Interação lógica para UIButtonHw.xam
    /// </summary>
    public partial class UIButtonHw : UserControl
    {
        private readonly IHardwareButton button;

        public UIButtonHw(string text, IHardwareButton button)
        {
            InitializeComponent();

            if (button is HwButton)
            {
                (button as HwButton).OnInit += UIButtonHw_OnInit;
                (button as HwButton).Halted += UIButtonHw_Halted;
            }

            btn.Content = $"{text} (0x{button.ID:X2})";
            this.button = button;
        }

        private void UIButtonHw_Halted()
        {
            Dispatcher.Invoke(() => btn.IsEnabled = false);
        }

        private void UIButtonHw_OnInit()
        {
            Dispatcher.Invoke(() => btn.IsEnabled = true);
        }

        private  void btn_Click(object sender, RoutedEventArgs e)
        {
            btn.IsEnabled = false;
            button.SendClick(() =>
            {
                Dispatcher.Invoke(() => btn.IsEnabled = true);
                return 0;
            });
        }

        private void HlpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"ID: 0x{button.ID.ToString("X2")} \nIntr. Click: {button.InterruptionCodeClick}", "Button Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
