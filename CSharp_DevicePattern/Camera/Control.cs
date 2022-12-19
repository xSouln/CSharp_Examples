using DevicePattern;
using Camera.UI;
using Camera.UI.Adapters;
using Camera.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib;
using xLib.Common;
using xLib.Transceiver;
using xLib.UI;
using xLib.Controls;
using xLib.Interfaces;
using xLib.Types;
using Camera.Communication.Transactions;

namespace Camera
{
    public class Control : DeviceBase, IConnectionStateReceiver
    {
        public Communication.Requests Requests;
        public Communication.Responses Responses;

        public Driver Driver;
        public Snapshot Snapshot;

        public Control(ITerminal terminal) : base(terminal)
        {
            Name = "Camera";

            Snapshot = new Snapshot(this);
            Driver = new Driver(this);

            Requests = new Communication.Requests(this);
            Responses = new Communication.Responses(this);

            ViewModel = new ControlViewModel(this);
        }

        protected override void StateUpdateHandler(xAction<bool, byte[]> transmitter)
        {
            //xTracer.Message(await Get.MotionState.Prepare(Requests.Handle).TransmitionAsync(transmitter, 1, 300), Name);
        }

        public override bool ResponseIdentification(xContent content)
        {
            if (Requests.Identification(content))
            {
                return true;
            }

            if (Responses.Identification(content))
            {
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            Driver.Dispose();
            Snapshot.Dispose();
        }

        public async void ConnectionStateListener(object context, ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Connected:
                    xTracer.Message(await Get.Options.Prepare(Requests.Handle).TransmitionAsync(Terminal.GetTransmitter(), 1, 300), Name);
                    break;
            }
        }
    }
}
