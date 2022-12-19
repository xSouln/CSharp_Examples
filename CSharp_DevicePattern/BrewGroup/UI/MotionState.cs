using BrewGroup.Communication.Transactions;
using BrewGroup.UI.Adapters;
using BrewGroup.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.Templates;
using xLib.UI;

namespace BrewGroup.UI
{
    public class MotionState : ModelBase<Control, MotionStateViewModel>
    {
        public UIProperty<float> TotalAngle = new UIProperty<float> { Name = nameof(TotalAngle) };
        public UIProperty<float, float> RequestAngle = new UIProperty<float, float> { Name = nameof(RequestAngle), RequestTemplateAdapter = new TemplateTextBox("Request") };

        public UIProperty<float, float> Speed = new UIProperty<float, float> { Name = nameof(Speed), RequestTemplateAdapter = new TemplateTextBox("Request") };
        public UIProperty<int, int> MoveTime = new UIProperty<int, int> { Name = nameof(MoveTime), RequestTemplateAdapter = new TemplateTextBox("Request") };
        public UIProperty<int> StepPosition = new UIProperty<int> { Name = nameof(StepPosition) };

        public MotionState(Control control) : base(control)
        {
            Name = nameof(MotionState);

            Get.MotionState.EventResponseReceive += EventResponseReceiveMotionState;
            Try.ResetPosition.EventResponseReceive += EventResponseReceiveClearPosition;

            ViewModel = new MotionStateViewModel(control, this);
        }

        private unsafe void EventResponseReceiveClearPosition(object obj, ResponseTryClearPosition arg)
        {
            TotalAngle.Value = arg.Position->TotalAngle;
            RequestAngle.Value = arg.Position->RequestAngle;
            StepPosition.Value = arg.Position->StepPosition;
        }

        public override void Dispose()
        {
            base.Dispose();

            Get.MotionState.EventResponseReceive -= EventResponseReceiveMotionState;
        }

        private void EventResponseReceiveMotionState(object obj, ResponseGetMotionStateT arg)
        {
            ResponseGetMotionState = arg;
        }

        protected ResponseGetMotionStateT ResponseGetMotionState
        {
            set
            {
                TotalAngle.Value = value.TotalAngle;
                RequestAngle.Value = value.RequestAngle;
                StepPosition.Value = value.StepPosition;

                Speed.Value = value.Speed;
                MoveTime.Value = value.MoveTime;
            }
        }

        public RequestSetPositionT RequestSetPosition => new RequestSetPositionT
        {
            Angle = RequestAngle.Request,
            Speed = Speed.Request,
            Time = MoveTime.Request
        };
    }
}
