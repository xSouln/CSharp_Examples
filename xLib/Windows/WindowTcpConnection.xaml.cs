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
using xLib.Net.UI.Models;
using xLib.Transceiver;

namespace xLib.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowTcpConnection.xaml
    /// </summary>
    public partial class WindowTcpConnection : Window
    {
        protected UIElement UIModel;

        public WindowTcpConnection()
        {
            InitializeComponent();
        }

        public WindowTcpConnection(Net.TCPClient tcp) : this()
        {
            if (tcp != null)
            {
                UIModel = new TcpClientViewModel(tcp).UIModel;

                Grid.SetColumn(UIModel, 0);
                Grid.SetRow(UIModel, 0);

                GridControl.Children.Add(UIModel);

                DataContext = UIModel;
            }
        }
    }
}
