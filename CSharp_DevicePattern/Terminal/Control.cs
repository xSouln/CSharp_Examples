using Camera.UI.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Terminal.UI.Models;
using xLib.Common;
using xLib.Controls;
using xLib.Interfaces;
using xLib.Net;
using xLib.Transceiver;
using xLib.Types;
using xLib.UI;

namespace Terminal
{
    public partial class Control : TerminalBase<ControlViewModel>
    {
        public Device.Control SomeDevice { get; set; }
        public Camera.Control Camera { get; set; }
        public Carousel.Control Carousel { get; set; }
        public Slider.Control Slider { get; set; }
        public BrewGroup.Control BrewGroup { get; set; }
        public FlowDirector.Control FlowDirector { get; set; }
        public CupsControl.Control RGBCups { get; set; }

        public Control(object parent)
        {
            Name = "Terminal";
            this.parent = parent;

            SomeDevice = new Device.Control(this);
            Camera = new Camera.Control(this);
            Carousel = new Carousel.Control(this);
            Slider = new Slider.Control(this);
            BrewGroup = new BrewGroup.Control(this);
            //FlowDirector = new FlowDirector.Control(this);
            RGBCups = new CupsControl.Control(this);

            AddDevice(SomeDevice);
            AddDevice(Camera);
            AddDevice(Carousel);
            AddDevice(Slider);
            AddDevice(BrewGroup);
            //AddDevice(FlowDirector);
            AddDevice(RGBCups);

            unsafe
            {
                TCP.Receiver.PacketReceiver += PacketReceiver;
                SerialPort.Receiver.PacketReceiver += PacketReceiver;
            }

            ViewModel = new ControlViewModel(this);

            TCP.ConnectionStateChangeEvent += ConnectionStateChangeEventHandler;
            SerialPort.ConnectionStateChangeEvent += ConnectionStateChangeEventHandler;
        }

        private void ConnectionStateChangeEventHandler(object context, ConnectionState state)
        {
            foreach (DeviceBase device in Devices)
            {
                if (device is IConnectionStateReceiver listener)
                {
                    listener.ConnectionStateListener(context, state);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            TCP.ConnectionStateChangeEvent -= ConnectionStateChangeEventHandler;
            SerialPort.ConnectionStateChangeEvent -= ConnectionStateChangeEventHandler;

            TCP?.Disconnect();
            SerialPort?.Disconnect();
        }
    }
}
