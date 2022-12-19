using FlowDirector.Communication.Transactions;
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
using FlowDirector.Types;
using System.Collections.ObjectModel;
using xLib.Windows;
using FlowDirector.UI.Models;

namespace FlowDirector.UI
{
    public class Options : ModelBase<Control, OptionsViewModel>
    {
        public Property<float> Acceleration = new Property<float>() { Name = nameof(Acceleration) };
        public Property<float> Deceleration = new Property<float>() { Name = nameof(Deceleration) };
        public Property<float> StartSpeed = new Property<float>() { Name = nameof(StartSpeed) };
        public Property<float> StopSpeed = new Property<float>() { Name = nameof(StopSpeed) };

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
                Deceleration = Deceleration.Value,
                StartSpeed = StartSpeed.Value,
                StopSpeed = StopSpeed.Value,
            };

            set
            {
                Acceleration.Value = value.Acceleration;
                Deceleration.Value = value.Deceleration;
                StartSpeed.Value = value.StartSpeed;
                StopSpeed.Value = value.StopSpeed;
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
