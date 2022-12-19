using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Types
{
    public delegate void ConnectionStateChangeEventHandler(object context, ConnectionState state);

    public enum ConnectionState
    {
        Disconnected,
        BeginConnect,
        Connecting,
        Connected,
        BeginDisconnect,
        Disconnecting,
    }
}
