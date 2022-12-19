using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Controls
{
    public interface ITerminal
    {
        string Name { get; set; }
        xAction<bool, byte[]> GetTransmitter();
        public void Dispose();
    }
}
