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
    public class ListViewAdapter : ListView
    {
        public GridView CustomView { get; set; }
        public ListViewAdapter(string items_source, string style, SelectionChangedEventHandler selection_changed) : base()
        {
            FontSize = 18;
            Margin = new Thickness { Left = 0, Right = 0, Bottom = 0, Top = 0 };
            Background = UIProperty.GetBrush("#FF3F3F46");
            Foreground = UIProperty.GetBrush("#FFDEC316");
            BorderBrush = UIProperty.GetBrush("#FF834545");
            VerticalAlignment = VerticalAlignment.Stretch;

            if (selection_changed != null)
            {
                SelectionChanged += selection_changed;
            }

            SetResourceReference(ListView.StyleProperty, style);
            SetBinding(ListView.ItemsSourceProperty, items_source);

            CustomView = new GridView();

            View = CustomView;
        }

        public ListViewAdapter(string items_source, SelectionChangedEventHandler selection_changed) : this(items_source, "ListViewStyle1", selection_changed)
        {

        }
    }
}
