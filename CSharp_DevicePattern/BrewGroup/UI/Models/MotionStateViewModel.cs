using BrewGroup.Communication.Transactions;
using BrewGroup.UI.Adapters;
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

namespace BrewGroup.UI.Models
{
    public class MotionStateViewModel : ViewModelBase<Control, MotionState>
    {
        public ObservableCollection<object> Propertys { get; set; }

        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };
        protected ListViewAdapter list_view;

        public MotionStateViewModel(Control control, MotionState parent) : base(control, parent)
        {
            Name = "MotionState";

            Propertys = new ObservableCollection<object>()
            {
                control.Status.Sensors.Sensor1,
                control.Status.Sensors.Sensor2,
                control.Status.MotionState,
                control.Status.MotionError,

                parent.TotalAngle,
                parent.RequestAngle,
                parent.Speed,
                parent.MoveTime,
                parent.StepPosition
            };

            buttons_grid.Add(new ButtonAdapter("Set position", SetPositionClickEvent));
            buttons_grid.Add(new ButtonAdapter("Stop", StopClickEvent));
            buttons_grid.Add(new ButtonAdapter("Reset position", ResetPositionClickEvent));

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
                Width = 180,
                DisplayMemberBinding = new Binding { Path = new PropertyPath("Name") }
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Value",
                CellTemplateSelector = new UITemplateSelector(),
                Width = 120
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Request",
                CellTemplateSelector = new UIRequestTemplateSelector(),
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

        private async void ResetPositionClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Try.ResetPosition.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void StopClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Try.Stop.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void SetPositionClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Set.Position.Prepare(Control.Requests.Handle, Parent.RequestSetPosition).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }
    }
}
