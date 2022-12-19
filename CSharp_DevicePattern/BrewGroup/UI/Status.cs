using BrewGroup.Communication.Transactions;
using BrewGroup.Types;
using BrewGroup.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.UI;

namespace BrewGroup.UI
{
    public class Status : ModelBase<Control, StatusViewModel>
    {
        public UIProperty<Types.MotionState> MotionState = new UIProperty<Types.MotionState> { Name = nameof(MotionState) };
        public UIProperty<MotionError> MotionError = new UIProperty<MotionError> { Name = nameof(MotionError) };

        public UISensors Sensors = new UISensors();

        protected StatusT status;

        public Status(Control control) : base(control)
        {
            Name = nameof(Status);

            Sensors = new UISensors();

            Get.MotionState.EventResponseReceive += EventResponseReceiveMotionState;
            Get.Status.EventResponseReceive += EventResponseReceiveStatus;
            Try.Stop.EventResponseReceive += EventResponseReceiveStop;

            ViewModel = new StatusViewModel(control, this);
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
            }
        }

        public class UISensors
        {
            public UIProperty<bool> Sensor1 = new UIProperty<bool> { Name = nameof(Sensor1) };
            public UIProperty<bool> Sensor2 = new UIProperty<bool> { Name = nameof(Sensor2) };

            public Sensors Value
            {
                set
                {
                    Sensor1.Value = (value & Types.Sensors.Sensor1) > 0;
                    Sensor2.Value = (value & Types.Sensors.Sensor2) > 0;
                }
            }
        }
    }
}
