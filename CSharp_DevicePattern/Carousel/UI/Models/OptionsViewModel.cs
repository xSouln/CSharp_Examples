﻿using Device.UI.Adapters;
using Carousel.Communication.Transactions;
using Carousel.Types;
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
using xLib.Common;
using Carousel.UI.Interfaces;

namespace Carousel.UI.Models
{
    public class OptionsViewModel : ViewModelBase<Control, Options>, IChangeSelector
    {
        public ObservableCollection<object> Propertys { get; set; }
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };

        protected ListViewAdapter list_view;

        public OptionsViewModel(Control control, Options parent) : base(control, parent)
        {
            Name = "Options";

            buttons_grid.Add(new ButtonAdapter("Get options", GetOptionsClickEvent));
            buttons_grid.Add(new ButtonAdapter("Set options", SetOptionsClickEvent));

            Propertys = new ObservableCollection<object>()
            {
                parent.Acceleration,
                parent.Deceleration,

                parent.StartSpeed,
                parent.StopSpeed,

                parent.Power
            };

            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            MenuItem menu_item_update = new MenuItem();
            menu_item_update.Header = "Update";
            menu_item_update.Click += MenuItemUpdateClick;

            MenuItem menu_item_copy_values_to_request = new MenuItem();
            menu_item_copy_values_to_request.Header = "Copy values to request";
            menu_item_copy_values_to_request.Click += MenuItemCopyValueToRequestClick;

            ContextMenu context_menu = new ContextMenu();
            context_menu.Items.Add(menu_item_update);
            context_menu.Items.Add(menu_item_copy_values_to_request);

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

        private void MenuItemCopyValueToRequestClick(object sender, RoutedEventArgs e)
        {
            Request = Control.Options.Value;
        }

        private async void MenuItemUpdateClick(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Get.Options.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected OptionsT Request
        {
            get => new OptionsT
            {
                Acceleration = Control.Options.Acceleration.Request,
                Deceleration = Control.Options.Deceleration.Request,

                StartSpeed = Control.Options.StartSpeed.Request,
                StopSpeed = Control.Options.StopSpeed.Request,

                Power = Control.Options.Power.Request,
            };
            set
            {
                Control.Options.Acceleration.Request = value.Acceleration;
                Control.Options.Deceleration.Request = value.Deceleration;

                Control.Options.StartSpeed.Request = value.StartSpeed;
                Control.Options.StopSpeed.Request = value.StopSpeed;

                Control.Options.Power.Request = value.Power;
            }
        }

        private async void GetOptionsClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Get.Options.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
        }

        private async void SetOptionsClickEvent(object sender, RoutedEventArgs e)
        {
            xTracer.Message(await Set.Options.Prepare(Control.Requests.Handle, Request).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 2000), Name);
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

        public async void Select(object context, object arg)
        {
            xTracer.Message(await Get.Options.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
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
