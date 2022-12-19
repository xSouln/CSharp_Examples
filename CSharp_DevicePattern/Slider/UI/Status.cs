using Slider.Communication.Transactions;
using Slider.Types;
using Slider.UI.Models;
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

namespace Slider.UI
{
    public class Status : ModelBase<Control, StatusViewModel>
    {
        public UIProperty<MotionState> MotionState = new UIProperty<MotionState> { Name = nameof(MotionState) };
        public UIProperty<MotionResult> MotionResult = new UIProperty<MotionResult> { Name = nameof(MotionResult) };
        public UIProperty<PodRealiseState> PodRealiseState = new UIProperty<PodRealiseState> { Name = nameof(PodRealiseState) };
        public UIProperty<PodRealiseResult> PodRealiseResult = new UIProperty<PodRealiseResult> { Name = nameof(PodRealiseResult) };
        public UIProperty<int> OpenTime = new UIProperty<int> { Name = "pod realise: " + nameof(OpenTime), TemplateAdapter = new TemplateTextBox("Value"), Value = 2000 };
        public UIProperty<float> Power = new UIProperty<float> { Name = "requst: " + nameof(Power), TemplateAdapter = new TemplateTextBox("Value"), Value = 50.0f };
        public UIProperty<int> TimeOut = new UIProperty<int> { Name = "requst: " + nameof(TimeOut), TemplateAdapter = new TemplateTextBox("Value"), Value = 2000 };
        public ModeProperty Mode = new ModeProperty { Name = "requst: " + nameof(Mode) };

        public UISensors Sensors = new UISensors();

        protected StatusT status;

        public Status(Control control) : base(control)
        {
            Name = nameof(Status);

            Sensors = new UISensors();

            Get.Status.EventResponseReceive += EventResponseReceiveStatus;
            Try.Close.EventResponseReceive += EventResponseReceiveTryClose;
            Try.Open.EventResponseReceive += EventResponseReceiveTryOpen;
            Try.Stop.EventResponseReceive += EventResponseReceiveTryStop;
            Try.SetPosition.EventResponseReceive += EventResponseReceiveTrySetPosition;
            Events.Close.EventReceive += EventReceiveClose;
            Events.Open.EventReceive += EventReceiveOpen;
            Events.Overcurrent.EventReceive += EventReceiveOvercurrent;
            Events.StatusChanged.EventReceive += EventReceiveStatusChanged;

            ViewModel = new StatusViewModel(control, this);
        }

        private unsafe void EventResponseReceiveTrySetPosition(object obj, Try.Response<StatusT> arg)
        {
            Value = *arg.Value;
        }

        private void EventReceiveStatusChanged(object obj, StatusT arg)
        {
            Value = arg;
        }

        private void EventReceiveOpen(object obj, StatusT arg)
        {
            Value = arg;
        }

        private void EventReceiveClose(object obj, StatusT arg)
        {
            Value = arg;
        }

        private void EventReceiveOvercurrent(object obj, StatusT arg)
        {
            Value = arg;
        }

        private void EventResponseReceiveStatus(object obj, StatusT arg)
        {
            Value = arg;
        }

        private unsafe void EventResponseReceiveTryStop(object obj, Try.Response<StatusT> arg)
        {
            Value = *arg.Value;
        }

        private unsafe void EventResponseReceiveTryOpen(object obj, Try.Response<StatusT> arg)
        {
            Value = *arg.Value;
        }

        private unsafe void EventResponseReceiveTryClose(object obj, Try.Response<StatusT> arg)
        {
            Value = *arg.Value;
        }

        public override void Dispose()
        {
            base.Dispose();

            Get.Status.EventResponseReceive -= EventResponseReceiveStatus;
            Try.Close.EventResponseReceive -= EventResponseReceiveTryClose;
            Try.Open.EventResponseReceive -= EventResponseReceiveTryOpen;
            Try.Stop.EventResponseReceive -= EventResponseReceiveTryStop;
            Events.Close.EventReceive -= EventReceiveClose;
            Events.Open.EventReceive -= EventReceiveOpen;
            Events.Overcurrent.EventReceive -= EventReceiveOvercurrent;
            Events.StatusChanged.EventReceive -= EventReceiveStatusChanged;
        }

        public StatusT Value
        {
            get => status;
            set
            {
                status = value;

                Sensors.Value = value.Sensors;
                MotionState.Value = value.Motor.State;
                MotionResult.Value = value.Motor.Result;
                PodRealiseState.Value = value.PodRealise.State;
                PodRealiseResult.Value = value.PodRealise.Result;
            }
        }

        public class UISensors
        {
            public UIProperty<bool> Close = new UIProperty<bool> { Name = "sensor: " + nameof(Close) };
            public UIProperty<bool> Open = new UIProperty<bool> { Name = "sensor: " + nameof(Open) };
            public UIProperty<bool> Overcurrent = new UIProperty<bool> { Name = "sensor: " + nameof(Overcurrent) };

            public SensorsStateT Value
            {
                set
                {
                    Close.Value = value.Close;
                    Open.Value = value.Open;
                    Overcurrent.Value = value.Overcurrent;
                }
            }
        }

        public class ModeProperty : UIProperty<MoveMode>
        {
            public class RequestTemplate : TemplateComboBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(ComboBox.SelectedValueProperty, new Binding { Path = new PropertyPath("Value") });
                    Element.SetValue(ComboBox.ItemsSourceProperty, new Binding { Path = new PropertyPath("Values") });
                }
            }

            public static IEnumerable<MoveMode> Values
            {
                get => Enum.GetValues(typeof(MoveMode)).Cast<MoveMode>();
                set { }
            }

            public ModeProperty()
            {
                this.TemplateAdapter = new RequestTemplate();
            }
        }
    }
}
