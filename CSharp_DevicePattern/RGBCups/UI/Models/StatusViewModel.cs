using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.UI;
using CupsControl.UI.Adapters;
using xLib.Templates;
using CupsControl.Types;
using xLib.Common;
using CupsControl.Communication.Transactions;

namespace CupsControl.UI.Models
{
    public class StatusViewModel : ViewModelBase<Control, Status>
    {
        public ObservableCollection<object> Propertys { get; set; }
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };

        public StatusViewModel(Control control, Status status) : base(control, status)
        {
            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            buttons_grid.Add(new ButtonAdapter("Set color", SetColorClickEvent));
            buttons_grid.Add(new ButtonAdapter("Set template by id", SetTemplateByIdClickEvent));

            var list_view = new ListViewAdapter(null, "ListViewStyle1", null);

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                Width = 200,
                //CellTemplateSelector = new UITemplateSelector(),
                DisplayMemberBinding = new Binding { Path = new PropertyPath("Name") }
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Value",
                CellTemplateSelector = new UITemplateSelector(),
                //DisplayMemberBinding = new Binding { Path = new PropertyPath("Value") },
                Width = 160
            });

            Propertys = new ObservableCollection<object>
            {
                status.DrawingIsEnable,

                new UIProperty<string> { TemplateAdapter = new TemplateContentControl() },
                status.Red,
                status.Green,
                status.Blue,

                new UIProperty<string> { TemplateAdapter = new TemplateContentControl() },
                status.TemplateId
            };

            list_view.ItemsSource = Propertys;

            Grid.SetColumn(list_view, 0);
            Grid.SetRow(list_view, 0);

            Grid.SetColumn(buttons_grid, 1);
            Grid.SetRow(buttons_grid, 0);

            grid.Children.Add(list_view);
            grid.Children.Add(buttons_grid);

            UIModel = grid;
        }

        private async void SetTemplateByIdClickEvent(object sender, RoutedEventArgs e)
        {
            RequestSetTemplateByIdT request = new RequestSetTemplateByIdT
            {
                Selector = (CupNumberBits)(1 << (int)Parent.Cup.Number),
                TemplateId = Parent.TemplateId.Value
            };
            xTracer.Message(await Set.TemplateById.Prepare(Control.Requests.Handle, request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 500), Control.Name);
        }

        private async void SetColorClickEvent(object sender, RoutedEventArgs e)
        {
            RequestSetColorT request = new RequestSetColorT
            {
                Selector = (CupNumberBits)(1 << (int)Parent.Cup.Number),
                Color = new ColorT
                {
                    Blue = Parent.Blue.Value,
                    Green = Parent.Green.Value,
                    Red = Parent.Red.Value,
                }
            };
            xTracer.Message(await Set.Color.Prepare(Control.Requests.Handle, request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 500), Control.Name);
        }
    }
}
