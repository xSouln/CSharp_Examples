using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Camera.UI.Adapters
{
    public class GridAdapter : Grid
    {
        public List<object> Elements { get; set; }

        public GridAdapter() : base()
        {
            Elements = new List<object>();

            Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 };
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grid.SetColumn(this, 0);
            Grid.SetRow(this, 0);
        }

        public void Add(UIElement element)
        {
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

            if (element != null && element is UIElement)
            {
                Grid.SetColumn(element, 0);
                Grid.SetRow(element, Elements.Count);
                Children.Add(element);
            }

            Elements.Add(element);
        }
    }

    public interface IGridAdapter
    {
        GridAdapter Grid { get; set; }
    }
}
