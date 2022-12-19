using Carousel.Communication.Transactions;
using Carousel.Types;
using Carousel.UI.Adapters;
using Carousel.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.Common;
using xLib.Templates;
using xLib.UI;

namespace Carousel.UI
{
    public class MotionState : ModelBase<Control, MotionStateViewModel>
    {
        public UIProperty<float> TotalAngle = new UIProperty<float> { Name = nameof(TotalAngle) };
        public UIProperty<float, float> RequestAngle = new UIProperty<float, float> { Name = nameof(RequestAngle), RequestTemplateAdapter = new TemplateTextBox("Request") };

        public UIProperty<float, float> Power = new UIProperty<float, float> { Name = nameof(Power), RequestTemplateAdapter = new TemplateTextBox("Request"), Request = 50 };
        public UIProperty<int, int> MoveTime = new UIProperty<int, int> { Name = nameof(MoveTime), RequestTemplateAdapter = new TemplateTextBox("Request"), Request = 5000 };
        public UIProperty<int> EncoderPosition = new UIProperty<int> { Name = nameof(EncoderPosition) };

        public UIProperty<short, short> Pod = new UIProperty<short, short> { Name = nameof(Pod), RequestTemplateAdapter = new TemplateTextBox("Request") };

        public ModeProperty MoveMode = new ModeProperty { Name = nameof(MoveMode) };

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
            EncoderPosition.Value = arg.Position->EncoderPosition;
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
                EncoderPosition.Value = value.EncoderPosition;

                Power.Value = value.Power;
                MoveTime.Value = value.MoveTime;
            }
        }

        public RequestSetPositionT RequestSetPosition => new RequestSetPositionT
        {
            Angle = RequestAngle.Request,
            Power = Power.Request,
            Time = MoveTime.Request,
            Mode = (uint)MoveMode.Request
        };

        public class ModeProperty : UIProperty<string, SetPositionMode>
        {
            public class RequestTemplate : TemplateComboBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(ComboBox.SelectedValueProperty, new Binding { Path = new PropertyPath("Request") });
                    Element.SetValue(ComboBox.ItemsSourceProperty, new Binding { Path = new PropertyPath("ERequests") });
                }
            }

            public static IEnumerable<SetPositionMode> ERequests
            {
                get => Enum.GetValues(typeof(SetPositionMode)).Cast<SetPositionMode>();
                set { }
            }

            public ModeProperty()
            {
                this.RequestTemplateAdapter = new RequestTemplate();
            }
        }
        /*
        public class ModeProperty : UIProperty<string, bool>
        {
            protected uint SetPositionModeMask = 0;
            protected SetPositionMode mode;

            public SetPositionMode Mode => (SetPositionMode)(SetPositionModeMask & (uint)mode);

            public ModeProperty(SetPositionMode mode)
            {
                Name = "" + mode;
                this.mode = mode;

                RequestTemplateAdapter = new TemplateButton("Request");
                RequestTemplateAdapter.Element.AddHandler(Button.ClickEvent, new RoutedEventHandler(Click));
            }

            private void Click(object sender, RoutedEventArgs e)
            {
                Request ^= true;

                if (Request)
                {
                    SetPositionModeMask = 0xffffffff;
                }
                else
                {
                    SetPositionModeMask = 0;
                }
            }
        }
        */
    }
}
