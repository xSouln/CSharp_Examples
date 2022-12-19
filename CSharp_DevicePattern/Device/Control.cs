using Device.Communication.Transactions;
using Device.UI.Models;
using DevicePattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib;
using xLib.Common;
using xLib.Controls;
using xLib.UI;

namespace Device
{
    public partial class Control : DeviceBase
    {
        public Communication.Requests Requests;
        public Communication.Responses Responses;

        public UI.Info Info;

        public Control(ITerminal terminal) : base(terminal)
        {
            Name = nameof(Device);

            Requests = new Communication.Requests(this);
            Responses = new Communication.Responses(this);

            Info = new UI.Info(this);

            ViewModel = new ControlViewModel(this);
        }

        protected override async void StateUpdateHandler(xAction<bool, byte[]> transmitter)
        {
            xTracer.Message(await Get.Time.Prepare(Requests.Handle).TransmitionAsync(transmitter, 1, 300), Name);
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
