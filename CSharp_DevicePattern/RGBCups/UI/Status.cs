using CupsControl.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Common;
using xLib.UI;
using CupsControl.Communication.Transactions;
using CupsControl.Types;
using xLib.Templates;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace CupsControl.UI
{
    public class Status : ModelBase<Control, StatusViewModel>
    {
        public UIProperty<bool> DrawingIsEnable = new UIProperty<bool> { Name = nameof(DrawingIsEnable) };

        public UIProperty<byte> Red = new UIProperty<byte> { Name = nameof(Red), TemplateAdapter = new TemplateTextBox("Value"), Value = 10 };
        public UIProperty<byte> Green = new UIProperty<byte> { Name = nameof(Green), TemplateAdapter = new TemplateTextBox("Value"), Value = 10 };
        public UIProperty<byte> Blue = new UIProperty<byte> { Name = nameof(Blue), TemplateAdapter = new TemplateTextBox("Value"), Value = 10 };
        public TemplateIdProperty TemplateId = new TemplateIdProperty { Name = nameof(TemplateId) };

        public Cup Cup { get; set; }

        public Status(Control control, Cup cup) : base(control)
        {
            Name = nameof(Status);
            Cup = cup;

            Get.Status.EventResponseReceive += EventResponseReceiveGetStatus;

            ViewModel = new StatusViewModel(control, this);
        }

        private unsafe void EventResponseReceiveGetStatus(object obj, ResponseGetStatus arg)
        {
            if ((int)Cup.Number < arg.ValuesCount)
            {
                DrawingIsEnable.Value = arg.Values[(int)Cup.Number].IsEnable(StatusBits.DrawingIsEnable);
            }
        }

        public class TemplateIdProperty : UIProperty<TemplateId>
        {
            public class RequestTemplate : TemplateComboBox
            {
                public RequestTemplate()
                {
                    Element.SetBinding(ComboBox.SelectedValueProperty, new Binding { Path = new PropertyPath("Value") });
                    Element.SetValue(ComboBox.ItemsSourceProperty, new Binding { Path = new PropertyPath("Values") });
                }
            }

            public static IEnumerable<TemplateId> Values
            {
                get => Enum.GetValues(typeof(TemplateId)).Cast<TemplateId>();
                set { }
            }

            public TemplateIdProperty()
            {
                this.TemplateAdapter = new RequestTemplate();
            }
        }
    }
}
