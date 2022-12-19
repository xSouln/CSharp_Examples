using Carousel.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.UI;
using Carousel.Types;
using xLib.Templates;
using Carousel.Communication.Transactions;

namespace Carousel.UI
{
    public class Calibration : ModelBase<Control, CalibrationViewModel>
    {
        public UIProperty<float, float> Position = new UIProperty<float, float>() { Name = nameof(Position), RequestTemplateAdapter = new TemplateTextBox("Request") };
        public UIProperty<float, float> Offset = new UIProperty<float, float> { Name = nameof(Offset), RequestTemplateAdapter = new TemplateTextBox("Request") };

        public Calibration(Control control) : base(control)
        {
            Name = nameof(Calibration);

            Get.Calibration.EventResponseReceive += EventResponseReceiveGetCalibration;
            Set.Calibration.EventResponseReceive += EventResponseReceiveSetCalibration;

            ViewModel = new CalibrationViewModel(control, this);
        }

        public override void Dispose()
        {
            base.Dispose();

            Get.Calibration.EventResponseReceive -= EventResponseReceiveGetCalibration;
            Set.Calibration.EventResponseReceive -= EventResponseReceiveSetCalibration;
        }

        private async void EventResponseReceiveSetCalibration(object obj, Set.Response arg)
        {
            if (arg.Result == Result.Accept)
            {
                xTracer.Message(await Get.Calibration.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300), Name);
            }
        }

        private void EventResponseReceiveGetCalibration(object obj, CalibrationT arg)
        {
            Value = arg;
        }

        public unsafe CalibrationT Value
        {
            get => new CalibrationT
            {
                Position = Position.Value,
                Offset = Offset.Value
            };
            set
            {
                Position.Value = (float)Math.Round(value.Position, 3);
                Offset.Value = (float)Math.Round(value.Offset, 3);
            }
        }
    }
}
