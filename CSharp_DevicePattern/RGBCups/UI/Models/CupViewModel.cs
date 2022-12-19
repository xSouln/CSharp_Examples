using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using xLib.UI;
using CupsControl.UI.Adapters;
using System.Windows.Data;
using xLib.Templates;
using CupsControl.Types;
using System.Collections.ObjectModel;

namespace CupsControl.UI.Models
{
    public class CupViewModel : ViewModelBase<Cup>
    {
        protected TabControl tab_control;

        public CupViewModel(Cup cup) : base(cup)
        {
            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition()); //{ Width = GridLength.Auto }

            tab_control = new TabControl()
            {
                TabStripPlacement = Dock.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 },
                BorderBrush = UIProperty.GetBrush("#FF834545"),
                Foreground = UIProperty.GetBrush("#FFDEC316"),
                Background = UIProperty.GetBrush("#FF3F3F46"),
                Width = double.NaN
            };

            tab_control.Items.Add(new TabItemAdapter("Status", cup.Status.ViewModel));

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
