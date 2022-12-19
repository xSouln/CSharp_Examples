using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Terminal.UI.Adapters;
using xLib.Controls;
using xLib.UI;

namespace Terminal.UI.Models
{
    public class ControlViewModel : ViewModelBase<Control>
    {
        protected TabControlAdapter tab_control;

        public ControlViewModel(Control control) : base(control)
        {
            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            tab_control = new TabControlAdapter()
            {
                TabStripPlacement = Dock.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 },
                BorderBrush = UIProperty.GetBrush("#FF834545"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                Background = UIProperty.GetBrush("#FF3F3F46"),
                Width = double.NaN
            };
            /*
            tab_control.Items.Add(new TabItemAdapter("Device", parent.SomeDevice.ViewModel));
            tab_control.Items.Add(new TabItemAdapter("Camera", parent.Camera.ViewModel));
            tab_control.Items.Add(new TabItemAdapter("Carousel", parent.Carousel.ViewModel));
            tab_control.Items.Add(new TabItemAdapter("RGBCups", parent.RGBCups.ViewModel));
            */

            foreach (DeviceBase device in control.Devices)
            {
                tab_control.Items.Add(new TabItemAdapter(device.Name, device.ViewModel));
            }

            Grid.SetColumn(tab_control, 0);
            Grid.SetRow(tab_control, 0);

            grid.Children.Add(tab_control);

            UIModel = grid;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected class TabItemAdapter : TabItem
        {
            public TabItemAdapter(string header, object data_context) : base()
            {
                Header = header;
                DataContext = data_context;
                FontSize = 18;
                Height = 120;
                BorderBrush = UIProperty.GetBrush("#FF834545");
                Foreground = UIProperty.GetBrush("#FFDEC316");
                Background = UIProperty.GetBrush("#FF3F3F46");
                Style = Application.Current.FindResource("TabItemStyle2") as Style;

                SetBinding(TabItem.ContentProperty, new Binding("UIModel"));
            }
        }
    }
}
