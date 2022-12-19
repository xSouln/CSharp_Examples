using CupsControl.UI.Models;
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
using xLib.Transceiver;
using xLib.UI;
using CupsControl.Communication.Transactions;

namespace CupsControl
{
    public partial class Control : DeviceBase
    {
        public Communication.Requests Requests;
        public Communication.Responses Responses;

        public UI.Cup[] Cups = new UI.Cup[(int)Types.Cups.Count];

        public Control(Terminal.Control parent) : base(parent)
        {
            Name = nameof(CupsControl);

            Requests = new Communication.Requests(this);
            Responses = new Communication.Responses(this);

            int i = 0;
            while (i < (int)Types.Cups.Count && i < Cups.Length)
            {
                Cups[i] = new UI.Cup(this, (Types.Cups)i);
                i++;
            }

            ViewModel = new ControlViewModel(this);
        }

        protected override async void StateUpdateHandler(xAction<bool, byte[]> transmitter)
        {
            xTracer.Message(await Get.Status.Prepare(Requests.Handle).TransmitionAsync(transmitter, 1, 300), Name);
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
        }
    }
}
