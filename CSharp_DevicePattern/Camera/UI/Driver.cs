using Camera.Communication;
using Camera.Communication.Transactions;
using Camera.UI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using xLib;
using xLib.Common;
using xLib.Transceiver;
using xLib.UI;

namespace Camera.UI
{
    public class Driver : ModelBase<Control, DriverViewModel>
    {
        public Options Options;
        public Configuration Configuration;

        public Driver(Control control) : base(control)
        {
            Name = "Driver";

            Options = new Options(this);
            Configuration = new Configuration(this);

            ViewModel = new DriverViewModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            Options.Dispose();
            Configuration.Dispose();
        }
    }
}
