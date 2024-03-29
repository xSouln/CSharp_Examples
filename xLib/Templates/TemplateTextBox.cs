﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.UI;

namespace xLib.Templates
{
    public class TemplateTextBox : UITemplateAdapter
    {
        protected void Init(string TextPropertyPath)
        {
            var grid = new FrameworkElementFactory(typeof(Grid));
            var free = new FrameworkElementFactory(typeof(FrameworkElement));
            Element = new FrameworkElementFactory(typeof(TextBox));

            grid.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            grid.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            free.SetValue(UIElement.VisibilityProperty, Visibility.Hidden);
            free.SetValue(FrameworkElement.WidthProperty, 200.0);
            free.SetBinding(FrameworkElement.HeightProperty, new Binding
            {
                RelativeSource = new RelativeSource
                {
                    Mode = RelativeSourceMode.TemplatedParent
                },
                Path = new PropertyPath("Height")
            });

            Element.SetValue(Control.FontSizeProperty, 18.0);
            Element.SetValue(Control.ForegroundProperty, UIProperty.GetBrush("#FFDEC316"));
            Element.SetValue(Control.BackgroundProperty, null);
            Element.SetValue(Control.BorderBrushProperty, UIProperty.GetBrush("#FF834545"));

            Element.SetValue(Control.PaddingProperty, new Thickness(-2));
            //Element.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));

            Element.SetValue(FrameworkElement.HeightProperty, double.NaN);
            Element.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            Element.SetValue(System.Windows.Controls.Primitives.TextBoxBase.CaretBrushProperty, UIProperty.GetBrush("#FFDEC316"));

            if (TextPropertyPath != null)
            {
                Element.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath(TextPropertyPath) });
            }

            grid.AppendChild(free);
            grid.AppendChild(Element);

            Template = new DataTemplate { VisualTree = grid };
        }

        public TemplateTextBox()
        {
            Init("Value");
        }

        public TemplateTextBox(string TextPropertyPath)// : base()
        {
            Init(TextPropertyPath);
        }
    }
}
