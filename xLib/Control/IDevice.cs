using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xLib.UI;

namespace xLib.Controls
{
    public interface IDevice
    {
        string Name { get; set; }
        ITerminal Terminal { get; set; }
        bool ResponseIdentification(xContent content);
        void Dispose();
    }
}
