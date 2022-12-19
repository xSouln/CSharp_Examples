using Device.Communication.Transactions;
using Device.UI.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.Common;
using xLib.UI;

namespace Device.UI.Models
{
    public class InfoViewModel : ViewModelBase<Control, Info>
    {
        public ObservableCollection<object> Propertys { get; set; }

        protected GridAdapter buttons_grid = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };
        protected ListViewAdapter list_view;

        public InfoViewModel(Info parent) : base(parent.Control, parent)
        {
            Propertys = new ObservableCollection<object>
            {
                parent.Firmware,
                parent.WorkTime
            };

            buttons_grid.Add(new ButtonAdapter("Reset time", ResetTimeClickEvent));
            buttons_grid.Add(new ButtonAdapter("Set time", SetTime_ClickEvent));

            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            list_view = new ListViewAdapter(nameof(Propertys), "ListViewStyle1", null);
            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding { Path = new PropertyPath("Name") },
                Width = 180
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Values",
                CellTemplateSelector = new UITemplateSelector(),
                Width = 120
            });

            Grid.SetColumn(list_view, 0);
            Grid.SetRow(list_view, 0);

            Grid.SetColumn(buttons_grid, 1);
            Grid.SetRow(buttons_grid, 0);

            grid.Children.Add(list_view);
            grid.Children.Add(buttons_grid);

            UIModel = grid;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private async void ResetTimeClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Try.ResetTime.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300);
            xTracer.Message("response time " + request.Name + ": " + request.ResponseTime);
        }

        private async void SetTime_ClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Set.Time.Prepare(Control.Requests.Handle, 500000).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300);
            xTracer.Message("response time " + request.Name + ": " + request.ResponseTime);
        }

        public class ButtonAdapter : Button
        {
            public ButtonAdapter(string content, RoutedEventHandler event_click) : base()
            {
                Content = content;
                FontSize = 18;
                Width = 150;
                Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 };
                Background = UIProperty.GetBrush("#FF4F4F4F");
                Foreground = UIProperty.GetBrush("#FFDEC316");
                VerticalAlignment = VerticalAlignment.Stretch;

                Click += event_click;
                this.SetResourceReference(System.Windows.Controls.Control.TemplateProperty, "ButtonTemplate1");
            }
        }
    }
}
