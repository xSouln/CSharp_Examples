using Carousel.Communication.Transactions;
using Carousel.Types;
using Carousel.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.UI;

namespace Carousel.UI
{
    public class Status : ModelBase<Control, StatusViewModel>
    {
        public UIProperty<Types.MotionState> MotionState = new UIProperty<Types.MotionState> { Name = nameof(MotionState) };
        public UIProperty<MotionError> MotionError = new UIProperty<MotionError> { Name = nameof(MotionError) };
        public UIProperty<CalibrationState> CalibrationState = new UIProperty<CalibrationState> { Name = nameof(CalibrationState) };
        public UIProperty<CalibrationStatus> CalibrationStatus = new UIProperty<CalibrationStatus> { Name = nameof(CalibrationStatus) };

        public UISensors Sensors = new UISensors();

        protected StatusT status;

        public Status(Control control) : base(control)
        {
            Name = nameof(Status);

            Sensors = new UISensors();

            Get.MotionState.EventResponseReceive += EventResponseReceiveMotionState;
            Get.Status.EventResponseReceive += EventResponseReceiveStatus;
            Try.Stop.EventResponseReceive += EventResponseReceiveStop;
            Events.StatusChanged.EventReceive += EventReceiveEventStatusChanged;

            ViewModel = new StatusViewModel(control, this);
        }

        private void EventReceiveEventStatusChanged(object obj, StatusT arg)
        {
            Value = arg;
        }

        private unsafe void EventResponseReceiveStop(object obj, ResponseTryStop arg)
        {
            Value = *arg.Status;
        }

        public override void Dispose()
        {
            base.Dispose();

            Get.MotionState.EventResponseReceive -= EventResponseReceiveMotionState;
            Get.Status.EventResponseReceive -= EventResponseReceiveStatus;
            Try.Stop.EventResponseReceive -= EventResponseReceiveStop;
        }

        private void EventResponseReceiveStatus(object obj, StatusT arg)
        {
            Value = arg;
        }

        private void EventResponseReceiveMotionState(object obj, ResponseGetMotionStateT arg)
        {
            Value = arg.Status;
        }

        public StatusT Value
        {
            get => status;
            set
            {
                status = value;

                Sensors.Value = value.Sensors;
                MotionState.Value = value.MotionState;
                MotionError.Value = value.MotionError;
                CalibrationState.Value = value.CalibrationState;
                CalibrationStatus.Value = value.Calibration;
            }
        }

        public class UISensors
        {
            public UIProperty<bool> ZeroMark = new UIProperty<bool> { Name = nameof(ZeroMark) };
            public UIProperty<bool> Overcurrent = new UIProperty<bool> { Name = nameof(Overcurrent) };

            public SensorsBits Value
            {
                set
                {
                    ZeroMark.Value = (value & SensorsBits.ZeroMark) > 0;
                    Overcurrent.Value = (value & SensorsBits.Overcurrent) > 0;
                }
            }
        }
    }
}
