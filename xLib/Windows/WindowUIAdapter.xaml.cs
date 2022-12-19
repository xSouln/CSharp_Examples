using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xLib.UI;

namespace xLib.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowUIAdapter.xaml
    /// </summary>
    public partial class WindowUIAdapter : Window
    {
        protected ViewModelBase view_model;

        public WindowUIAdapter()
        {
            InitializeComponent();
        }

        public WindowUIAdapter(ViewModelBase view_model) : this()
        {
            if (view_model != null)
            {
                this.view_model = view_model.Clone();

                Grid.SetColumn(this.view_model.UIModel, 0);
                Grid.SetRow(this.view_model.UIModel, 0);

                GridControl.Children.Add(this.view_model.UIModel);

                DataContext = view_model;
            }
        }
    }
}
