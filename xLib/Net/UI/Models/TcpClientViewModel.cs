using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using xLib.Interfaces;
using xLib.Types;
using xLib.UI;

namespace xLib.Net.UI.Models
{
    public class TcpClientViewModel : ViewModelBase<TCPClient>
    {
        protected Brush background = UIProperty.RED;
        protected string address;
        protected int port = 5000;
        protected string button_content = "Connect";
        protected ConnectionState connection_state;

        protected Grid grid;
        protected Button button;
        protected TextBox text_box_address;
        protected TextBox text_box_port;
        protected Label label_address;
        protected Label label_port;
        protected Label label_status;
        protected TextBox text_box_status;

        public TcpClientViewModel(TCPClient control) : base(control)
        {
            control.PropertyChanged += ParentPropertyChanged;

            grid = new Grid();
            grid.Margin = new Thickness { Bottom = 2, Left = 2, Right = 2, Top = 2 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            button = new Button()
            {
                Content = "Connect",
                FontSize = 20,
                Width = 150,
                Height = 40,
                Margin = new Thickness { Left = 2, Right = 2, Bottom = 10, Top = 10 },
                Background = UIProperty.GetBrush("#FF4F4F4F"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
            };
            button.SetBinding(Button.ContentProperty, new Binding(nameof(ButtonContent)) { Mode = BindingMode.OneWay });
            button.SetResourceReference(Button.TemplateProperty, "ButtonTemplate1");
            button.DataContext = this;
            button.Click += Button_ClickEvent;

            text_box_address = new TextBox()
            {
                FontSize = 20,
                Width = 150,
                Height = 35,
                Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 },
                Background = UIProperty.GetBrush("#FF4F4F4F"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                BorderBrush = UIProperty.GetBrush("#FF834545"),
            };

            text_box_address.SetBinding(TextBox.TextProperty, new Binding(nameof(Address)));
            //text_box_address.SetBinding(TextBox.BackgroundProperty, new Binding(nameof(Background)));

            text_box_port = new TextBox()
            {
                FontSize = 20,
                Width = 150,
                Height = 35,
                Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 },
                Background = UIProperty.GetBrush("#FF4F4F4F"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                BorderBrush = UIProperty.GetBrush("#FF834545"),
            };

            text_box_port.SetBinding(TextBox.TextProperty, new Binding(nameof(Port)));
            //text_box_port.SetBinding(TextBox.BackgroundProperty, new Binding(nameof(Background)));

            label_address = new Label()
            {
                Content = "address",
                FontSize = 20,
                Width = 120,
                Height = 35,
                Foreground = UIProperty.GetBrush("#FFDEC316"),
            };

            label_port = new Label()
            {
                Content = "port",
                FontSize = 20,
                Width = 120,
                Height = 35,
                Foreground = UIProperty.GetBrush("#FFDEC316"),
            };

            label_status = new Label()
            {
                Content = "status",
                FontSize = 20,
                Width = 120,
                Height = 35,
                Foreground = UIProperty.GetBrush("#FFDEC316"),
            };

            text_box_status = new TextBox()
            {
                FontSize = 20,
                Width = 150,
                Height = 35,
                Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 },
                Background = UIProperty.GetBrush("#FF4F4F4F"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                BorderBrush = UIProperty.GetBrush("#FF834545"),
                IsReadOnly = true
            };

            text_box_status.SetBinding(TextBox.TextProperty, new Binding(nameof(ConnectionState)));
            text_box_status.SetBinding(TextBox.BackgroundProperty, new Binding(nameof(Background)));
            text_box_status.DataContext = this;

            Grid.SetColumn(label_address, 0);
            Grid.SetRow(label_address, 1);

            Grid.SetColumn(label_port, 0);
            Grid.SetRow(label_port, 2);

            Grid.SetColumn(text_box_address, 1);
            Grid.SetRow(text_box_address, 1);

            Grid.SetColumn(text_box_port, 1);
            Grid.SetRow(text_box_port, 2);

            Grid.SetColumn(button, 1);
            Grid.SetRow(button, 3);

            Grid.SetColumn(label_status, 0);
            Grid.SetRow(label_status, 0);

            Grid.SetColumn(text_box_status, 1);
            Grid.SetRow(text_box_status, 0);

            grid.Children.Add(label_status);
            grid.Children.Add(text_box_status);
            grid.Children.Add(label_address);
            grid.Children.Add(label_port);
            grid.Children.Add(text_box_address);
            grid.Children.Add(text_box_port);
            grid.Children.Add(button);

            grid.DataContext = this;

            SetAddress(control.Address);
            ConnectionState = control.ConnectionState;

            UIModel = grid;
        }

        private void ParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TCPClient parent = (TCPClient)sender;

            switch (e.PropertyName)
            {
                case nameof(parent.Address):
                    SetAddress(parent.Address);
                    break;

                case nameof(parent.ConnectionState):
                    ConnectionState = parent.ConnectionState;
                    //connection_state = parent.ConnectionState;
                    //OnPropertyChanged(nameof(ConnectionState));
                    break;
            }
        }

        public override ViewModelBase Clone()
        {
            var result = new TcpClientViewModel(Control);
            result.SetAddress(Control.Address);
            return result;
        }

        public ConnectionState ConnectionState
        {
            get => connection_state;
            set
            {
                switch (value)
                {
                    case ConnectionState.Disconnected:
                        background = UIProperty.RED;
                        button_content = "Connect";
                        break;

                    case ConnectionState.Connecting:
                        background = UIProperty.YELLOW;
                        button_content = "Break";
                        break;

                    case ConnectionState.Connected:
                        background = UIProperty.GREEN;
                        button_content = "Disconnect";
                        break;
                }

                connection_state = value;
                OnPropertyChanged(nameof(Background));
                OnPropertyChanged(nameof(ConnectionState));
                OnPropertyChanged(nameof(ButtonContent));
            }
        }

        public string ButtonContent => button_content;

        protected void Button_ClickEvent(object sender, RoutedEventArgs e)
        {
            switch (Control.ConnectionState)
            {
                case ConnectionState.Disconnected:
                    Control.Connect(Address + ":" + port);
                    break;

                case ConnectionState.Connecting:
                    Control.Disconnect();
                    break;

                case ConnectionState.Connected:
                    Control.Disconnect();
                    break;
            }
        }

        public Brush Background => background;

        protected void SetAddress(string address)
        {
            if (address != null)
            {
                string[] list = address.Split(':');

                if (list.Length == 2)
                {
                    Address = list[0];
                    Port = int.Parse(list[1]);
                }
            }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
