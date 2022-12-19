using Device.Communication;
using Device.Communication.Transactions;
using Device.UI.Adapters;
using Device.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using xLib;
using xLib.Common;
using xLib.Templates;
using xLib.UI;

namespace Device.UI
{
    public class Info : ModelBase<Control, InfoViewModel>
    {
        public UIProperty<string> Firmware = new UIProperty<string> { Name = nameof(Firmware) };

        public UIProperty<int> WorkTime = new UIProperty<int> { Name = "Work time" };

        public Info(Control control) : base(control)
        {
            Try.ResetTime.EventResponseReceive += ResetTimeEventReceive;
            Set.Time.EventResponseReceive += SetTimeEventReceive;
            Get.Time.EventResponseReceive += TimeEventReceive;

            ViewModel = new InfoViewModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected unsafe void TimeEventReceive(object obj, Get.Response<int> arg)
        {
            WorkTime.Value = *arg.Values;
        }

        protected unsafe void ResetTimeEventReceive(object obj, Try.ResponseResult<ActionResult, int> arg)
        {
            WorkTime.Value = *arg.Values;
        }

        protected unsafe void SetTimeEventReceive(object obj, Set.ResponseResult<int> arg)
        {
            WorkTime.Value = *arg.Values;
        }

        public class Property<TValue, TRequest> : UIProperty<TValue, TRequest>
        {
            public static readonly RequestTemplateProperty RequestTemplate = new RequestTemplateProperty();
            public static readonly UITemplateAdapter Template = new TemplateContentControl("Value");

            public class RequestTemplateProperty : TemplateTextBox
            {
                public RequestTemplateProperty() : base()
                {
                    base.Element.SetBinding(System.Windows.Controls.Control.BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
                    Element.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath("Request") });
                }
            }

            public Property() : base()
            {
                TemplateAdapter = Template;
                RequestTemplateAdapter = RequestTemplate;
            }

            public Brush Background { get; set; } = (Brush)new BrushConverter().ConvertFrom("#FF641818");
        }
    }
}
