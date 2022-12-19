using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLib.Types;
using xLib.UI;
using static xLib.Net.TCPClient;

namespace xLib.Net
{
    public class TCPClientBase : UINotifyPropertyChanged
    {
        public xEvent<TCPClient> EventDisconnected;
        public xEvent<TCPClient> EventConnected;

        public xAction<string> Tracer;
        protected TcpClient client;
        protected NetworkStream stream;
        protected Thread client_thread;

        protected string address;

        public string Ip;
        public int Port;
        protected ConnectionState connection_state;

        public int UpdatePeriod = 1;
        protected int transmit_deadtime = 1;
    }
}
