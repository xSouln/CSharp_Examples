using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.UI;

namespace Device.UI.Models
{
    public class ControlViewModel : ViewModelBase<Control>
    {
        protected TabControl tab_control;

        public ControlViewModel(Control parent) : base(parent)
        {
            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition()); //{ Width = GridLength.Auto }

            tab_control = new TabControl()
            {
                TabStripPlacement = Dock.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 },
                BorderBrush = null,// UIProperty.GetBrush("#FF834545"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                Background = UIProperty.GetBrush("#FF3F3F46"),
                Width = double.NaN
            };

            tab_control.Items.Add(new TabItemAdapter("Info", parent.Info.ViewModel));

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
                Height = 33;
                Width = 150;
                BorderBrush = UIProperty.GetBrush("#FF834545");
                Foreground = UIProperty.GetBrush("#FFDEC316");
                Background = UIProperty.GetBrush("#FF3F3F46");
                Style = Application.Current.FindResource("TabItemStyle1") as Style;

                SetBinding(TabItem.ContentProperty, new Binding("UIModel"));
            }
        }
    }
}
