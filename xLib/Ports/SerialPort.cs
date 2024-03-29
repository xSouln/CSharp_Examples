﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using xLib.Common;
using xLib.Interfaces;
using xLib.Transceiver;
using xLib.Types;
using xLib.UI;

namespace xLib.Ports
{
    public partial class xSerialPort : UINotifyPropertyChanged
    {
        public class UI_ButtonConnection : UINotifyPropertyChanged
        {
            private const string name_connected = "Disconnect";
            private const string name_disconnected = "Connect";

            private static Brush background_disconnected = UIProperty.RED;
            private static Brush background_connected = UIProperty.GREEN;

            private Brush background = background_disconnected;
            private string name = name_disconnected;

            private bool state;

            public string Name => name;

            public Brush Background => background;

            public bool IsEnabled
            {
                set
                {
                    if (state != value)
                    {
                        state = value;

                        if (value)
                        {
                            background = background_connected;
                            name = name_connected;
                        }
                        else
                        {
                            background = background_disconnected;
                            name = name_disconnected;
                        }

                        OnPropertyChanged(nameof(Name));
                        OnPropertyChanged(nameof(Background));
                    }
                }
            }
        }

        public xAction<string> Tracer;

        public event xEventChangeState<xSerialPort, string> EventFindeLastConnectedPortName;

        public event ConnectionStateChangeEventHandler ConnectionStateChangeEvent;

        private Thread RxThread;
        private Timer timer_finde_ports;

        public System.IO.Ports.SerialPort Port;
        public ObservableCollection<string> PortList { get; set; } = new ObservableCollection<string>();

        protected bool is_connected;
        protected int boad_rate = 115200;
        protected string port_name = "";
        protected string last_selected_port_name = "";
        protected Brush background_state = UIProperty.RED;

        protected AutoResetEvent transmition_synchronize = new AutoResetEvent(true);

        public UI_ButtonConnection ButtonConnection { get; set; } = new UI_ButtonConnection();

        public xObjectReceiver Receiver;

        public List<int> BaudRateList { get; set; } = new List<int>() { 9600, 38400, 115200, 128000, 256000, 521600, 840000, 900000, 921600 };

        public xSerialPort()
        {
            timer_finde_ports = new Timer(finde_ports, null, 1000, 1000);
        }

        private void trace(string note)
        {
            //Tracer?.Invoke(note);
            xTracer.Message(note);
        }

        public bool SelectIsEnable => !IsConnected;

        public Options SerialPortOptions
        {
            get => new Options
            {
                BoadRate = boad_rate,
                LastConnectedPortName = last_selected_port_name,
                ConnectionState = IsConnected
            };
            set
            {
                if (value != null)
                {
                    BoadRate = value.BoadRate;
                    PortName = value.LastConnectedPortName;
                }
            }
        }

        public Brush BackgroundState
        {
            get => background_state;
            set
            {
                background_state = value;
                OnPropertyChanged(nameof(BackgroundState));
            }
        }

        public bool IsConnected
        {
            get => is_connected;
            set
            {
                if (value && background_state != UIProperty.GREEN)
                {
                    BackgroundState = UIProperty.GREEN;
                }
                else if (!value && background_state != UIProperty.RED)
                {
                    BackgroundState = UIProperty.RED;
                }

                if (is_connected != value)
                {
                    is_connected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(SelectIsEnable));

                    if (is_connected)
                    {
                        ConnectionStateChangeEvent?.Invoke(this, ConnectionState.Connected);
                    }
                    else
                    {
                        ConnectionStateChangeEvent?.Invoke(this, ConnectionState.Disconnected);
                    }
                }

                ButtonConnection.IsEnabled = value;
            }
        }

        public string PortName
        {
            get => port_name;
            set
            {
                port_name = value;
                if (port_name.Length > 0)
                {
                    last_selected_port_name = value;
                }
                OnPropertyChanged(nameof(PortName));
            }
        }

        public int BoadRate
        {
            get => boad_rate;
            set
            {
                if (boad_rate != value)
                {
                    if (Port != null)
                    {
                        Port.BaudRate = value;
                    }
                    boad_rate = value;
                    trace("" + PortName + "(boad rate changed at " + boad_rate + ")");
                    OnPropertyChanged(nameof(BoadRate));
                }
            }
        }

        private void read_data()
        {
            while (Port != null && Port.IsOpen)
            {
                while (Port.BytesToRead > 0)
                {
                    Receiver.Add((byte)Port.ReadByte());
                }
                Thread.Sleep(1);
            }

            trace(PortName + "(boadrate: " + BoadRate + "): error read data");
            Disconnect();
        }

        public bool Connect(string name)
        {
            if (name == null || name.Length < 3)
            {
                return false;
            }

            if (Port != null && Port.IsOpen)
            {
                return false;
            }

            try
            {
                if (Receiver == null)
                {
                    Receiver = new xObjectReceiver(50000, new byte[] { (byte)'\r', (byte)'\n' });
                }

                Port = new System.IO.Ports.SerialPort(name, BoadRate, Parity.None, 8, StopBits.One);
                Port.Encoding = Encoding.GetEncoding("iso-8859-1");
                Port.ReadBufferSize = 100000;
                Port.WriteBufferSize = 100000;

                Port.Open();
                PortName = name;
                Receiver.Clear();
                trace(name + "(boadrate: " + BoadRate + "): rx thred started");

                IsConnected = true;
                //timer_update_rx = new Timer(read_data, this, 1000, 10);
                RxThread = new Thread(read_data);
                RxThread.Start();
                trace(name + "(boadrate: " + BoadRate + "): connected");
            }
            catch (Exception ex)
            {
                trace(name + "(boadrate: " + BoadRate + "): error connect " + ex);
                Disconnect();
            }

            return IsConnected;
        }

        public void Disconnect()
        {
            //timer_update_rx?.Dispose();

            RxThread?.Abort();
            RxThread = null;

            Port?.Close();
            Port = null;

            IsConnected = false;
            trace(PortName + "(boadrate: " + BoadRate + "): disconnected");
        }

        public void Dispose() { timer_finde_ports.Dispose(); Disconnect(); }

        private static void update_port_list(object arg)
        {
            (ObservableCollection<string> ports, List<string> remove, List<string> add) res = ((ObservableCollection<string> ports, List<string> remove, List<string> add))arg;

            foreach (string name in res.remove)
            {
                int i = 0;
                while (i < res.ports.Count)
                {
                    if (xConverter.Compare(name, res.ports[i]))
                    {
                        res.ports.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            foreach (string name in res.add)
            {
                res.ports.Add(name);
            }
        }

        private void finde_ports(object obj)
        {
            List<string> Ports = SerialPort.GetPortNames().ToList<string>();
            List<string> TotalPorts = new List<string>();

            int count = TotalPorts.Count;

            foreach (string name in PortList)
            {
                TotalPorts.Add(name);
                if (PortName.Length == 0 && xConverter.Compare(last_selected_port_name, name))
                {
                    PortName = name;
                    trace(PortName + "(finde last selected port name)");
                    EventFindeLastConnectedPortName?.Invoke(this, name);
                }
            }

            int i = 0;
            while (i < TotalPorts.Count && Ports.Count > 0)
            {
                int j = 0;
                while (j < Ports.Count)
                {
                    if (xConverter.Compare(TotalPorts[i], Ports[j]))
                    {
                        TotalPorts.RemoveAt(i);
                        Ports.RemoveAt(j);
                        goto end_while;
                    }
                    j++;
                }
                i++;
            end_while:;
            }

            if (TotalPorts.Count != Ports.Count || count != TotalPorts.Count)
            {
                xSupport.ActionThreadUI<(ObservableCollection<string>, List<string>, List<string>)>(update_port_list, (PortList, TotalPorts, Ports));
            }
        }

        public bool Send(string str)
        {
            if (Port == null || !Port.IsOpen || str == null || str.Length == 0)
            {
                return false;
            }
            Port.Write(str);
            return true;
        }

        public bool Send(byte[] data)
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (Port != null && Port.IsOpen && data != null && data.Length > 0)
                {
                    Port.Write(data, 0, data.Length);
                    return true;
                }
            }
            finally
            {
                transmition_synchronize.Set();
            }
            return false;
        }
    }
}
