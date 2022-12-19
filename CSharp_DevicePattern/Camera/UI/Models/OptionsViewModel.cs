using Device.UI.Adapters;
using Camera.Communication.Transactions;
using Camera.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.Transceiver;
using xLib.UI;
using xLib.Windows;

namespace Camera.UI.Models
{
    public class OptionsViewModel : ViewModelBase<Control, Options>
    {
        public ObservableCollection<object> Propertys { get; set; }
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };

        protected ListViewAdapter list_view;
        protected static WindowUIAdapter window_ui_adapter;

        public OptionsViewModel(Control control, Options parent) : base(control, parent)
        {
            buttons_grid.Add(new ButtonAdapter("Get options", GetOptionsClickEvent));
            buttons_grid.Add(new ButtonAdapter("Set options", SetOptionsClickEvent));

            Propertys = new ObservableCollection<object>()
            {
                parent.OutputFormat,
                parent.Resolution,

                parent.Brightness,
                parent.Contrast,
                parent.Saturation,

                parent.LightMode,
                parent.SpecialEffect,
                parent.Quantization,

                parent.AGC_Gain,
            };

            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            MenuItem menu_item_open_in_new_window = new MenuItem();
            menu_item_open_in_new_window.Header = "Open in new window";
            menu_item_open_in_new_window.Click += MenuItemOpenInNewWindowClick;

            ContextMenu context_menu = new ContextMenu();
            context_menu.Items.Add(menu_item_open_in_new_window);

            list_view = new ListViewAdapter(null, "ListViewStyle1", null)
            {
                ContextMenu = context_menu
            };

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

        public override void Dispose()
        {
            window_ui_adapter?.Close();
            window_ui_adapter = null;
        }

        protected OptionsT Request => new OptionsT
        {
            OutputFormat = Parent.OutputFormat.Request,
            Resolution = Parent.Resolution.Request,

            Brightness = Parent.Brightness.Request,
            Contrast = Parent.Contrast.Request,
            Saturation = Parent.Saturation.Request,

            LightMode = Parent.LightMode.Request,
            SpecialEffect = Parent.SpecialEffect.Request,
            Quantization = Parent.Quantization.Request,
            AGC_Gain = Parent.AGC_Gain.Request,
        };

        private async void GetOptionsClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Get.Options.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300);
            TransmitterBase.Trace(nameof(GetOptionsClickEvent), request);
        }

        private async void SetOptionsClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Set.Options.Prepare(Control.Requests.Handle, Request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 2000);
            TransmitterBase.Trace(nameof(SetOptionsClickEvent), request);
        }

        private void MenuItemOpenInNewWindowClick(object sender, RoutedEventArgs e)
        {
            if (window_ui_adapter == null)
            {
                window_ui_adapter = new WindowUIAdapter(this)
                {
                    Title = Control.Name + " " + Parent.Name + " " + Name,
                    Background = UIProperty.GetBrush("#FF3F3F46")
                };

                window_ui_adapter.Closed += (event_sender, event_e) =>
                {
                    window_ui_adapter?.Close();
                    window_ui_adapter = null;
                };

                window_ui_adapter.Show();
            }
            else
            {
                window_ui_adapter.Activate();
            }
        }

        public override void Update()
        {
            OnPropertyChanged(nameof(UIModel));
            OnPropertyChanged(nameof(Propertys));
            OnPropertyChanged(nameof(Name));
        }

        public override object GetParent()
        {
            return Parent;
        }

        public override ViewModelBase Clone()
        {
            return new OptionsViewModel(Control, Parent);
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
                this.SetResourceReference(Button.TemplateProperty, "ButtonTemplate1");
            }
        }
    }
}
