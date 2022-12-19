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
using xLib;
using xLib.Common;
using xLib.UI;
using Device.Properties;
using xLib.Windows;

namespace DevicePattern
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected WindowTcpConnection window_tcp_connection { get; set; }

        public Terminal.Control Terminal;

        public MainWindow()
        {
            xSupport.Context = this;

            InitializeComponent();

            MenuTcp.Click += MenuTcpClick;
            MenuSerialPort.Click += WindowSerialPort.OpenClick;
            MenuTerminal.Click += WindowTerminal.OpenClick;

            Terminal = new Terminal.Control(this);
            Terminal.SerialPort.SerialPortOptions = Settings.Default.SerialPortOptions;
            Terminal.TCP.Address = Settings.Default.TcpLastAddres;

            MenuSerialPort.DataContext = Terminal.SerialPort;
            MenuTcp.DataContext = Terminal.TCP;

            WindowSerialPort.SerialPort = Terminal.SerialPort;

            Grid.SetColumn(Terminal.ViewModel.UIModel, 0);
            Grid.SetRow(Terminal.ViewModel.UIModel, 0);

            GridControl.Children.Add(Terminal.ViewModel.UIModel);

            Closed += MainWindow_Closed;
        }

        private void MenuTcpClick(object sender, RoutedEventArgs e)
        {
            if (window_tcp_connection == null)
            {
                window_tcp_connection = new WindowTcpConnection(Terminal.TCP);
                window_tcp_connection.Closed += (arg_sender, arg_e) =>
                {
                    window_tcp_connection?.Close();
                    window_tcp_connection = null;
                };
                window_tcp_connection.Show();
            }
            else
            {
                window_tcp_connection.Activate();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.TcpLastAddres = Terminal?.TCP.Address;
            Settings.Default.SerialPortOptions = Terminal.SerialPort.SerialPortOptions;
            Settings.Default.Save();

            Terminal.Dispose();

            WindowTerminal.Dispose();
            WindowSerialPort.Dispose();

            window_tcp_connection?.Close();
        }
    }
}
