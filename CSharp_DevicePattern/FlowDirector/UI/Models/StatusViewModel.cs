using FlowDirector.UI.Adapters;
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

namespace FlowDirector.UI.Models
{
    public class StatusViewModel : ViewModelBase<Control, Status>
    {
        public ObservableCollection<object> Propertys { get; set; }

        protected ListViewAdapter list_view;

        public StatusViewModel(Control control, Status parent) : base(control, parent)
        {
            Name = "Status";

            Propertys = new ObservableCollection<object>()
            {
                parent.Sensors.Sensor1,
                parent.Sensors.Sensor2,

                parent.MotionState,
                parent.MotionError,
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
                Width = 180,
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

            grid.Children.Add(list_view);

            UIModel = grid;
        }
    }
}
