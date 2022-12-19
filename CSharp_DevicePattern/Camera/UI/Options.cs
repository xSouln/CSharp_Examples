using Camera.Communication.Transactions;
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
using Camera.Communication;
using xLib;
using xLib.Common;
using Camera.Types;
using Camera.UI.Adapters;
using System.Collections.ObjectModel;
using xLib.Windows;
using Camera.UI.Models;

namespace Camera.UI
{
    public class Options : ModelBase<Control, OptionsViewModel>
    {
        public Property<OutputFormats> OutputFormat = new Property<OutputFormats>() { Name = "Output format" };

        public Property<Resolutions> Resolution = new Property<Resolutions>() { Name = "Resolution", Request = Resolutions._1280x960 };

        public Property<Brightnesses> Brightness = new Property<Brightnesses>() { Name = "Brightness" };
        public Property<Contrasts> Contrast = new Property<Contrasts>() { Name = "Contrast" };
        public Property<Saturations> Saturation = new Property<Saturations>() { Name = "Saturation" };

        public Property<LightModes> LightMode = new Property<LightModes>() { Name = "Light mode", Value = LightModes.Home };
        public Property<SpecialEffects> SpecialEffect = new Property<SpecialEffects>() { Name = "Special effect" };
        public PropertyQuantization Quantization = new PropertyQuantization() { Name = "Quantization", Request = (byte)Quantizations.Default };
        public Property<byte, byte> AGC_Gain = new Property<byte, byte>() { Name = "AGC gain" };


        public Options(Driver parent) : base(parent.Control)
        {
            Get.Options.EventResponseReceive += GetOptionsEventReceive;
            Set.Options.EventResponseReceive += SetOptionsEventReceive;

            ViewModel = new OptionsViewModel(parent.Control, this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public OptionsT Value
        {
            get => new OptionsT
            {
                OutputFormat = OutputFormat.Value,
                Resolution = Resolution.Value,

                Brightness = Brightness.Value,
                Contrast = Contrast.Value,
                Saturation = Saturation.Value,

                LightMode = LightMode.Value,
                SpecialEffect = SpecialEffect.Value,
                Quantization = Quantization.Value,

                AGC_Gain = AGC_Gain.Value
            };

            set
            {
                OutputFormat.Value = value.OutputFormat;
                Resolution.Value = value.Resolution;

                Brightness.Value = value.Brightness;
                Contrast.Value = value.Contrast;
                Saturation.Value = value.Saturation;

                LightMode.Value = value.LightMode;
                SpecialEffect.Value = value.SpecialEffect;
                Quantization.Value = value.Quantization;

                AGC_Gain.Value = value.AGC_Gain;
            }
        }

        protected unsafe void SetOptionsEventReceive(object obj, Set.ResponseResult<OptionsT> arg)
        {
            xTracer.Message("" + ((TransmitterBase)obj).Name + " " + arg.Result);

            if (arg.Result == ActionResult.ACCEPT)
            {
                Value = arg.Values[0];
            }
        }

        protected unsafe void GetOptionsEventReceive(object obj, Get.ResponseResult<OptionsT> arg)
        {
            Value = arg.Values[0];
        }

        public class Property<T> : UIProperty<T, T>
        {
            public class RequestTemplate : TemplateComboBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(ComboBox.SelectedValueProperty, new Binding { Path = new PropertyPath("Request") });
                    Element.SetValue(ComboBox.ItemsSourceProperty, new Binding { Path = new PropertyPath("ERequests") });
                }
            }

            public static IEnumerable<T> ERequests
            {
                get => Enum.GetValues(typeof(T)).Cast<T>();
                set { }
            }

            public Property()
            {
                this.RequestTemplateAdapter = new RequestTemplate();
            }
        }

        public class Property<TValue, TRequest> : UIProperty<TValue, TRequest> where TValue : unmanaged where TRequest : unmanaged
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
                _request = default(byte);

                this.RequestTemplateAdapter = new RequestTemplate();
            }
        }

        public class PropertyQuantization : UIProperty<byte, byte>
        {
            public class RequestTemplate : TemplateTextBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath("Request") });
                }
            }

            public override byte Request
            {
                get => (byte)_request;
                set
                {
                    int total = (byte)_request;
                    int request = value & 0x3f;

                    if (total != request)
                    {
                        _request = (byte)request;

                        ValueUpdate();
                        ValueChanged();
                    }
                }
            }

            public PropertyQuantization()
            {
                _request = default(byte);

                this.RequestTemplateAdapter = new RequestTemplate();
            }
        }
    }
}
