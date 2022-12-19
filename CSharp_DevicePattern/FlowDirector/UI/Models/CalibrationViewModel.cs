using FlowDirector.Communication.Transactions;
using FlowDirector.Types;
using FlowDirector.UI.Adapters;
using FlowDirector.UI.Interfaces;
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

namespace FlowDirector.UI.Models
{
    public class CalibrationViewModel : ViewModelBase<Control, Calibration>, IChangeSelector
    {
        public ObservableCollection<object> Propertys { get; set; }
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };

        protected ListViewAdapter list_view;

        public CalibrationViewModel(Control control, Calibration parent) : base(control, parent)
        {
            Name = nameof(CalibrationViewModel);

            buttons_grid.Add(new ButtonAdapter("Get calibration", GetCalibrationClickEvent));
            buttons_grid.Add(new ButtonAdapter("Set calibration", SetCalibrationClickEvent));

            Propertys = new ObservableCollection<object>()
            {
                parent.Position
            };

            foreach (UIProperty property in parent.Speed)
            {
                Propertys.Add(property);
            }

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

        private async void SetCalibrationClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Set.Calibration.Prepare(Control.Requests.Handle, Request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        protected unsafe CalibrationT Request
        {
            get
            {
                SpeedCalibrationT speed = new SpeedCalibrationT();

                for (int i = 0; i < (int)SpeedCalibrationInfo.PolynomialSize; i++)
                {
                    speed.Polynomial[i] = Parent.Speed[i].Request;
                }

                return new CalibrationT()
                {
                    Position = Parent.Position.Request,
                    Speed = speed
                };
            }
        }

        private async void GetCalibrationClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Get.Calibration.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        public async void Select(object context, object arg)
        {
            xTracer.Message(await Get.Calibration.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }
    }
}
