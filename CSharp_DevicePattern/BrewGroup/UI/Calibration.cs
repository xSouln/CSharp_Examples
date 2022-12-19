using BrewGroup.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.UI;
using BrewGroup.Types;
using xLib.Templates;
using BrewGroup.Communication.Transactions;

namespace BrewGroup.UI
{
    public class Calibration : ModelBase<Control, CalibrationViewModel>
    {
        public UIProperty<float, float> Position = new UIProperty<float, float>() { Name = nameof(Position), RequestTemplateAdapter = new TemplateTextBox("Request") };
        public UIProperty<float, float>[] Speed = new UIProperty<float, float>[(int)SpeedCalibrationInfo.PolynomialSize];

        public Calibration(Control control) : base(control)
        {
            Name = nameof(Calibration);

            for (int i = 0; i < (int)SpeedCalibrationInfo.PolynomialSize; i++)
            {
                Speed[i] = new UIProperty<float, float>
                {
                    Name = nameof(Speed) + " K: " + i,
                    RequestTemplateAdapter = new TemplateTextBox("Request")
                };
            }

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
            get
            {
                SpeedCalibrationT speed_calibration = new SpeedCalibrationT();

                for (int i = 0; i < (int)SpeedCalibrationInfo.PolynomialSize; i++)
                {
                    speed_calibration.Polynomial[i] = (float)Math.Round(Speed[i].Value, 3);
                }

                return new CalibrationT
                {
                    Position = (float)Math.Round(Position.Value, 3),
                    Speed = speed_calibration
                };
            }

            set
            {
                for (int i = 0; i < (int)SpeedCalibrationInfo.PolynomialSize; i++)
                {
                    Speed[i].Value = (float)Math.Round(value.Speed.Polynomial[i], 3);
                    Position.Value = (float)Math.Round(value.Position, 3);
                }
            }
        }
    }
}
