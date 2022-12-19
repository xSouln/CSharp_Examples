using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using xLib;
using xLib.Common;
using xLib.Interfaces;
using xLib.Net.UI.Models;
using xLib.Types;
using xLib.UI;

namespace xLib.Net
{
    public class TCPClient : UINotifyPropertyChanged
    {
        public xAction<string> Tracer;
        protected TcpClient client;
        protected NetworkStream stream;
        protected Thread client_thread;

        protected string address;

        public string Ip;
        public int Port;
        protected ConnectionState connection_state;

        public event ConnectionStateChangeEventHandler ConnectionStateChangeEvent;

        public int UpdatePeriod = 1;
        protected int transmit_deadtime = 1;

        protected List<byte[]> transmit_line = new List<byte[]>();
        protected AutoResetEvent transmit_line_synchronizer = new AutoResetEvent(true);
        protected Thread transmit_line_thread;

        public TcpClientViewModel ViewModel;

        public xObjectReceiver Receiver;

        private void trace(string note)
        {
            Tracer?.Invoke(note);
            xTracer.Message(note);
        }

        public TCPClient()
        {
            ViewModel = new TcpClientViewModel(this);
        }

        public int TransmitDeadtime
        {
            get => transmit_deadtime;
            set
            {
                if (value > 1)
                {
                    transmit_deadtime = value;
                }
            }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;

                OnPropertyChanged(nameof(Address));
            }
        }

        public ConnectionState ConnectionState
        {
            get => connection_state;
            set
            {
                if (connection_state != value)
                {
                    connection_state = value;

                    ConnectionStateChangeEvent?.Invoke(this, connection_state);
                    OnPropertyChanged(nameof(ConnectionState));
                }
            }
        }

        private void rx_thread()
        {
            if (client == null)
            {
                trace("tcp client: client == null");
                thread_close();
            }
            try
            {
                client.ReceiveBufferSize = 0x10000;
                stream = client.GetStream();
                stream.Flush();
                ConnectionState = ConnectionState.Connected;
                trace("tcp client: thread start");
                client.ReceiveBufferSize = 1000000;

                int count = 0;
                byte[] buf = new byte[1000000];
                Receiver.Clear();

                while (true)
                {
                    do
                    {
                        count = stream.Read(buf, 0, buf.Length);
                        for (int i = 0; i < count; i++)
                        {
                            Receiver.Add(buf[i]);
                        }
                    }
                    while ((bool)stream?.DataAvailable);
                }
            }
            catch (Exception e)
            {
                trace(e.ToString());
                thread_close();
            }
        }

        private void thread_close()
        {
            try
            {
                client_thread?.Abort();
                transmit_line_thread?.Abort();

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                    stream = null;
                }

                if (client != null)
                {
                    client.Client?.Close();
                    client.Close();
                    client = null;
                }
            }
            finally
            {
                trace("tcp client: thread close");
                transmit_line.Clear();
                client_thread = null;
                transmit_line_thread = null;
                ConnectionState = ConnectionState.Disconnected;
            }
        }

        private void request_callback(IAsyncResult ar)
        {
            try
            {
                TcpClient result = (TcpClient)ar.AsyncState;
                if (result != null && result.Client != null)
                {
                    trace("tcp: client connected");
                    client = result;
                    client_thread = new Thread(new ThreadStart(rx_thread));
                    client_thread.Start();
                }
                else
                {
                    trace("tcp client: client connect error");
                }
            }
            catch (Exception ex)
            {
                trace(ex.ToString());
                trace("tcp client: client connect abort");
                thread_close();
                return;
            }
        }

        public void Connect(string address)
        {
            string[] strs;

            if (address == null) { trace("tcp client: address == null"); return; }

            if (client != null && client.Connected) { trace("tcp client: device is connected"); return; }
            trace("tcp client: request connect");

            if (address.Length < 9) { trace("tcp client: incorrect parameters"); return; }
            strs = address.Split('.');
            if (strs.Length < 4) { trace("tcp client: incorrect parameters"); return; }

            strs = address.Split(':');
            if (strs.Length != 2) { trace("tcp client: incorrect parameters"); return; }

            int port = Convert.ToInt32(strs[1]);
            string ip = strs[0];

            Ip = ip;
            Port = port;
            client = new TcpClient();

            Address = address;

            ConnectionState = ConnectionState.Connecting;

            trace("tcp client: client begin connect");
            IAsyncResult result = client.BeginConnect(Ip, Port, request_callback, client);
        }
        //=====================================================================================================================
        public void Disconnect()
        {
            trace("tcp client: request disconnect");
            thread_close();
        }
        //=====================================================================================================================
        public bool Send(string str)
        {
            if (client != null && stream != null && client.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(str + "\r");
                trace("tcp client send: " + str);

                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp client: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }

        public bool Send(byte[] data)
        {
            if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
            {
                try { stream.Write(data, 0, data.Length); return true; }
                catch { trace("tcp client: невозможно отправить на указаный ip"); return false; }
            }
            //trace("tcp: нет соединения");
            return false;
        }
        //=====================================================================================================================
        private void transmit_line_thread_handler()
        {
            while (true)
            {
                if (transmit_line.Count > 0)
                {
                    Send(transmit_line[0]);

                    transmit_line_synchronizer.WaitOne();
                    transmit_line.RemoveAt(0);
                    transmit_line_synchronizer.Set();
                }
                Thread.Sleep(transmit_deadtime);
            }
        }

        public bool InLineAdd(byte[] data)
        {
            if (client != null && stream != null && client.Connected && data != null && data.Length > 0)
            {
                try
                {
                    transmit_line_synchronizer.WaitOne();
                    transmit_line.Add(data);
                    return true;
                }
                finally
                {
                    transmit_line_synchronizer.Set();
                }
            }
            return false;
        }
        //=====================================================================================================================
    }
}
