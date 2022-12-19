using CupsControl.Types;
using CupsControl.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;

namespace CupsControl.UI
{
    public class Cup : ModelBase<Control, CupViewModel>
    {
        public Status Status;

        public Cups Number { get; set; }

        public Cup(Control control, Cups cup) : base(control)
        {
            Number = cup;

            Status = new Status(control, this);

            ViewModel = new CupViewModel(this);
        }
    }
}
