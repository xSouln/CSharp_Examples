using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI;

namespace Device.Carousel.UI.Models
{
    public class DeviceControlViewModel : ViewModelBase<Control>
    {
        public ObservableCollection<object> Propertys { get; set; }

        public DeviceControlViewModel(Control control) : base(control)
        {

        }
    }
}
