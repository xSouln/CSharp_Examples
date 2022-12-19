using Slider.Communication.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.Templates;
using xLib.Transceiver;
using xLib.UI;
using xLib;
using xLib.Common;
using Slider.Types;
using System.Collections.ObjectModel;
using xLib.Windows;
using Slider.UI.Models;

namespace Slider.UI
{
    public class Options : ModelBase<Control, OptionsViewModel>
    {
        public Property<float> Acceleration = new Property<float>() { Name = nameof(Acceleration) };
        public Property<float> StartSpeed = new Property<float>() { Name = nameof(StartSpeed) };
        public Property<float> Power = new Property<float>() { Name = nameof(Power) };

        public Options(Control control) : base(control)
        {
            Name = nameof(Options);

            Get.Options.EventResponseReceive += GetOptionsEventReceive;
            Set.Options.EventResponseReceive += SetOptionsEventReceive;

            ViewModel = new OptionsViewModel(control, this);
        }

        public override void Dispose()
        {
            base.Dispose();

            Get.Options.EventResponseReceive -= GetOptionsEventReceive;
            Set.Options.EventResponseReceive -= SetOptionsEventReceive;
        }

        public OptionsT Value
        {
            get => new OptionsT
            {
                Acceleration = Acceleration.Value,
                StartSpeed = StartSpeed.Value,
                Power = Power.Value,
            };

            set
            {
                Acceleration.Value = value.Acceleration;
                StartSpeed.Value = value.StartSpeed;
                Power.Value = value.Power;
            }
        }

        protected unsafe void SetOptionsEventReceive(object obj, Set.Response<OptionsT> arg)
        {
            xTracer.Message("" + ((TransmitterBase)obj).Name + " " + arg.Result);

            if (arg.Result == Result.Accept)
            {
                Value = arg.Values[0];
            }
        }

        protected unsafe void GetOptionsEventReceive(object obj, OptionsT arg)
        {
            Value = arg;
        }

        public class Property<T> : UIProperty<T, T> where T : unmanaged
        {
            public class RequestTemplate : TemplateTextBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath("Request") });
                }
            }

            public Property()
            {
                _request = default(T);
                _value = default(T);

                this.RequestTemplateAdapter = new RequestTemplate();
            }
        }
    }
}
