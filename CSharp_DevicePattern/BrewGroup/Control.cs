using BrewGroup.Communication.Transactions;
using BrewGroup.UI.Models;
using DevicePattern;
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
using xLib.Controls;
using xLib.Interfaces;
using xLib.Transceiver;
using xLib.Types;
using xLib.UI;

namespace BrewGroup
{
    public class Control : DeviceBase, IConnectionStateReceiver
    {
        public Communication.Requests Requests;
        public Communication.Responses Responses;

        public UI.Options Options;
        public UI.Status Status;
        public UI.MotionState MotionState;
        public UI.Calibration Calibration;

        public Control(TerminalBase terminal) : base(terminal)
        {
            Name = "Brew group";

            Requests = new Communication.Requests(this);
            Responses = new Communication.Responses(this);

            Options = new UI.Options(this);
            Status = new UI.Status(this);
            MotionState = new UI.MotionState(this);
            Calibration = new UI.Calibration(this);

            ViewModel = new ControlViewModel(this);
        }

        protected override async void StateUpdateHandler(xAction<bool, byte[]> transmitter)
        {
            xTracer.Message(await Get.MotionState.Prepare(Requests.Handle).TransmitionAsync(transmitter, 1, 300), Name);
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

            Options.Dispose();
        }

        public async void ConnectionStateListener(object context, ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Connected:
                    xTracer.Message(await Get.Options.Prepare(Requests.Handle).TransmitionAsync(Terminal.GetTransmitter(), 1, 300), Name);
                    xTracer.Message(await Get.Calibration.Prepare(Requests.Handle).TransmitionAsync(Terminal.GetTransmitter(), 1, 300), Name);
                    break;
            }
        }
    }
}
