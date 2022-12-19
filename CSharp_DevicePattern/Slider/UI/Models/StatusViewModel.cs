using Slider.UI.Adapters;
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
using Slider.Communication.Transactions;
using xLib.UI;
using Slider.UI.Interfaces;

namespace Slider.UI.Models
{
    public class StatusViewModel : ViewModelBase<Control, Status>
    {
        public ObservableCollection<object> Propertys { get; set; }
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };
        protected ListViewAdapter list_view;

        public StatusViewModel(Control control, Status parent) : base(control, parent)
        {
            Name = "Status";

            buttons_grid.Add(new ButtonAdapter("Open", OpenClickEvent));
            buttons_grid.Add(new ButtonAdapter("Close", CloseClickEvent));
            buttons_grid.Add(null);
            buttons_grid.Add(new ButtonAdapter("Drop pod", DropPodClickEvent));
            buttons_grid.Add(null);
            buttons_grid.Add(new ButtonAdapter("Stop", StopClickEvent));

            Propertys = new ObservableCollection<object>()
            {
                parent.Sensors.Open,
                parent.Sensors.Close,
                parent.Sensors.Overcurrent,
                new UIProperty<string> { Value = "" },
                parent.MotionState,
                parent.MotionResult,
                new UIProperty<string> { Value = "" },
                parent.PodRealiseState,
                parent.PodRealiseResult,
                new UIProperty<string> { Value = "" },
                parent.Power,
                parent.TimeOut,
                parent.Mode,
                new UIProperty<string> { Value = "" },
                parent.OpenTime
            };

            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            list_view = new ListViewAdapter(null, "ListViewStyle1", null);
            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                Width = 200,
                DisplayMemberBinding = new Binding { Path = new PropertyPath("Name") }
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Value",
                CellTemplateSelector = new UITemplateSelector(),
                Width = 120
            });

            list_view.ItemsSource = Propertys;

            Grid.SetColumn(list_view, 0);
            Grid.SetRow(list_view, 0);

            Grid.SetColumn(buttons_grid, 1);
            Grid.SetRow(buttons_grid, 0);

            grid.Children.Add(list_view);
            grid.Children.Add(buttons_grid);

            UIModel = grid;
        }

        private async void DropPodClickEvent(object sender, RoutedEventArgs e)
        {
            RequestTryDropPodT request = new RequestTryDropPodT
            {
                OpenTime = Parent.OpenTime.Value,
            };

            xTracer.Message(await Try.DropePode.Prepare(Control.Requests.Handle, request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void StopClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Try.Stop.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void CloseClickEvent(object sender, RoutedEventArgs e)
        {
            RequestTrySetPositionT request = new RequestTrySetPositionT
            {
                Power = Parent.Power.Value,
                TimeOut = Parent.TimeOut.Value,
                Mode = Parent.Mode.Value,
                Position = Types.MovePosition.Close
            };

            xTracer.Message(await Try.SetPosition.Prepare(Control.Requests.Handle, request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void OpenClickEvent(object sender, RoutedEventArgs e)
        {
            RequestTrySetPositionT request = new RequestTrySetPositionT
            {
                Power = Parent.Power.Value,
                TimeOut = Parent.TimeOut.Value,
                Mode = Parent.Mode.Value,
                Position = Types.MovePosition.Open
            };

            xTracer.Message(await Try.SetPosition.Prepare(Control.Requests.Handle, request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }
    }
}
