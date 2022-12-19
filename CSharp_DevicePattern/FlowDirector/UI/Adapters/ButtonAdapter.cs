using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using xLib.UI;

namespace FlowDirector.UI.Adapters
{
    public class ButtonAdapter : Button
    {
        public ButtonAdapter(string content, string template, RoutedEventHandler event_click) : base()
        {
            Content = content;
            FontSize = 18;
            Width = 150;
            Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 };
            Background = UIProperty.GetBrush("#FF4F4F4F");
            Foreground = UIProperty.GetBrush("#FFDEC316");

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            this.SetResourceReference(Button.TemplateProperty, template);

            Click += event_click;
        }

        public ButtonAdapter(string content, RoutedEventHandler event_click) : this(content, "ButtonTemplate1", event_click)
        {

        }
    }
}
