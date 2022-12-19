using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Types;

namespace xLib.Interfaces
{
    public interface IConnectionStateReceiver
    {
        void ConnectionStateListener(object context, ConnectionState state);
    }
}
