using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BrewGroup.UI.Adapters
{
    public class ImageAdapter : Image
    {
        public ImageAdapter(string source)
        {
            Margin = new Thickness { Left = 2, Right = 2, Bottom = 2, Top = 2 };

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            this.SetBinding(Image.SourceProperty, source);
        }
    }
}
