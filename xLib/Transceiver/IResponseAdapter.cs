using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public interface IResponseAdapter
    {
        public object Recieve(xContent content);
    }
}
