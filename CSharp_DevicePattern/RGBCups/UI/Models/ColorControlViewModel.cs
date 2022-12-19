using CupsControl.Communication.Transactions;
using CupsControl.Types;
using CupsControl.UI.Adapters;
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
using xLib.Templates;
using xLib.UI;

namespace CupsControl.UI.Models
{
    public class ColorControlViewModel : ViewModelBase<Control>
    {
        public ObservableCollection<object> Propertys { get; set; }

        public struct ElementT
        {
            public CupNumberBits Selector { get; set; }
            public byte Green { get; set; }
            public byte Red { get; set; }
            public byte Blue { get; set; }
        }

        public ColorControlViewModel(Control control) : base(control)
        {
            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition()); //{ Width = GridLength.Auto }

            var list_view = new ListViewAdapter(null, "ListViewStyle1", null);

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                Width = 90,
                DisplayMemberBinding = new Binding { Path = new PropertyPath("Selector") }
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Green",
                CellTemplate = new TemplateTextBox("Green").Template,
                Width = 90
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Red",
                CellTemplate = new TemplateTextBox("Red").Template,
                Width = 90
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Blue",
                CellTemplate = new TemplateTextBox("Blue").Template,
                Width = 90
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "",
                CellTemplate = new ColorTemplateButton(Click).Template,
                Width = 90
            });

            Propertys = new ObservableCollection<object>
            {
                new ElementT { Selector = CupNumberBits.Cup1, Blue = 0x10, Green = 0x10, Red = 0x10 },
                new ElementT { Selector = CupNumberBits.Cup2, Blue = 0x10, Green = 0x10, Red = 0x10 },
                new ElementT { Selector = CupNumberBits.Cup3, Blue = 0x10, Green = 0x10, Red = 0x10 },
                new ElementT { Selector = CupNumberBits.Cup4, Blue = 0x10, Green = 0x10, Red = 0x10 },
            };

            list_view.ItemsSource = Propertys;

            Grid.SetColumn(list_view, 0);
            Grid.SetRow(list_view, 0);

            grid.Children.Add(list_view);

            UIModel = grid;
        }

        private async void Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null && button.DataContext != null && button.DataContext is ElementT pixel)
            {
                RequestSetColorT color = new RequestSetColorT
                {
                    Selector = pixel.Selector,
                    Color = new ColorT
                    {
                        Blue = pixel.Blue,
                        Green = pixel.Green,
                        Red = pixel.Red
                    }
                };

                var result = await Set.Color.Prepare(Control.Requests.Handle, color).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 500);
                xTracer.Message(result, Control.Name);
            }
        }

        protected class ColorTemplateButton : TemplateButton
        {
            public ColorTemplateButton(RoutedEventHandler handler)
            {
                Element.SetValue(Button.ContentProperty, "Set");
                Element.AddHandler(Button.ClickEvent, handler);
            }
        }
    }
}
