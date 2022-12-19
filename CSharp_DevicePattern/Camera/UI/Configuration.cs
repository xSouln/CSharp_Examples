using Camera.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;

namespace Camera.UI
{
    public class Configuration : ModelBase<Control, ConfigurationViewModel>
    {
        public Configuration(Driver parent) : base(parent.Control)
        {
            ViewModel = new ConfigurationViewModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}