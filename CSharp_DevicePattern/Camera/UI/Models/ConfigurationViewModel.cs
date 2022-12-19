using Device.Common;
using Microsoft.Win32;
using Camera.Communication.Transactions;
using Camera.UI.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLib.Templates;
using xLib.Transceiver;
using xLib.UI;

namespace Camera.UI.Models
{
    public class ConfigurationViewModel : ViewModelBase<Control, Configuration>
    {
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };
        public ObservableCollection<Element> List { get; set; } = new ObservableCollection<Element>();

        protected ListViewAdapter list_view;

        public ConfigurationViewModel(Configuration parent) : base(parent.Control, parent)
        {
            buttons_grid.Add(new ButtonAdapter("Save list", SavePropertysClickEvent));
            buttons_grid.Add(new ButtonAdapter("Open list", OpenPropertysClickEvent));
            buttons_grid.Add(null);

            buttons_grid.Add(new ButtonAdapter("Add property", AddPropertyClickEvent));
            buttons_grid.Add(new ButtonAdapter("Clear propertys", ClearPropertysClickEvent));
            buttons_grid.Add(new ButtonAdapter("Remove property", RemovePropertyClickEvent));
            buttons_grid.Add(new ButtonAdapter("Insert property", InsertPropertyClickEvent));
            buttons_grid.Add(null);

            buttons_grid.Add(new ButtonAdapter("Apply propertys", ApplyPropertysClickEvent));

            var grid = new Grid();
            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            list_view = new ListViewAdapter(nameof(List), "ListViewStyle1", null);
            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Register",
                CellTemplateSelector = new Element.TemplateSelectorRegister(),
                Width = 120
            });

            list_view.CustomView.Columns.Add(new GridViewColumn
            {
                Header = "Values",
                CellTemplateSelector = new Element.TemplateSelectorValue(),
                Width = 120
            });

            Grid.SetColumn(list_view, 0);
            Grid.SetRow(list_view, 0);

            Grid.SetColumn(buttons_grid, 1);
            Grid.SetRow(buttons_grid, 0);

            grid.Children.Add(list_view);
            grid.Children.Add(buttons_grid);

            UIModel = grid;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void OpenPropertysClickEvent(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            Element.WordT[] list;

            if (OPF.ShowDialog() == true && Json.Open(OPF.FileName, out list))
            {
                List.Clear();

                for (int i = 0; i < list.Length; i++)
                {
                    List.Add(new Element { Word = list[i] });
                }
            }
        }

        private void SavePropertysClickEvent(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.FileName = "configuration";

            if (SFD.ShowDialog() == true)
            {
                string path = Json.FileExtensionClear(SFD.FileName);
                Json.Save(path, Element.ToArray(List.ToArray()));
            }
        }

        private void InsertPropertyClickEvent(object sender, RoutedEventArgs e)
        {
        }

        private async void ApplyPropertysClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Set.Configuration.Prepare(Control.Requests.Handle, Element.ToByteArray(List.ToArray()))?.TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 2000);
            TransmitterBase.Trace(nameof(ApplyPropertysClickEvent), request);
        }

        private void RemovePropertyClickEvent(object sender, RoutedEventArgs e)
        {
            if (list_view.SelectedValue != null)
            {
                List.Remove((Element)list_view.SelectedValue);
            }
        }

        private void ClearPropertysClickEvent(object sender, RoutedEventArgs e)
        {
            List.Clear();
        }

        private void AddPropertyClickEvent(object sender, RoutedEventArgs e)
        {
            List.Add(new Element());
        }

        public class Element : UINotifyPropertyChanged
        {
            public UITemplateAdapter RegisterTemplateAdapter { get; set; }
            public UITemplateAdapter ValueTemplateAdapter { get; set; }

            protected string _register;
            protected string _value;

            [Serializable]
            public struct WordT
            {
                public string Register { get; set; }
                public string Value { get; set; }
            }

            public WordT Word
            {
                get => new WordT()
                {
                    Register = _register,
                    Value = _value
                };
                set
                {
                    Register = value.Register;
                    Value = value.Value;
                }
            }

            public string Register
            {
                get => _register;
                set
                {
                    if (_register != value)
                    {
                        _register = value;
                        OnPropertyChanged(nameof(Register));
                    }
                }
            }

            public string Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }

            public class Template : TemplateTextBox
            {
                public Template(string binding_path)
                {
                    Element.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath(binding_path) });
                }
            }

            public Element()
            {
                RegisterTemplateAdapter = new Template("Register");
                ValueTemplateAdapter = new Template("Value");
            }

            public class TemplateSelectorRegister : DataTemplateSelector
            {
                public override DataTemplate SelectTemplate(object item, DependencyObject container)
                {
                    return item != null && item is Element element ? element.RegisterTemplateAdapter.Template : null;
                }
            }

            public class TemplateSelectorValue : DataTemplateSelector
            {
                public override DataTemplate SelectTemplate(object item, DependencyObject container)
                {
                    return item != null && item is Element element ? element.ValueTemplateAdapter.Template : null;
                }
            }

            public static WordT[] ToArray(Element[] elements)
            {
                if (elements != null && elements.Length > 0)
                {
                    List<WordT> result = new List<WordT>();
                    try
                    {
                        foreach (Element element in elements)
                        {
                            result.Add(element.Word);
                        }
                        return result.ToArray();
                    }
                    catch { }
                }

                return null;
            }

            public static byte[] ToByteArray(Element[] elements)
            {
                if (elements != null && elements.Length > 0)
                {
                    byte[] array = new byte[elements.Length * 2];
                    int i = 0;
                    try
                    {
                        foreach (Element element in elements)
                        {
                            array[i++] = Convert.ToByte(element.Register, 16);
                            array[i++] = Convert.ToByte(element.Value, 16);
                        }
                        return array;
                    }
                    catch
                    {
                        return null;
                    }
                }

                return null;
            }
        }
    }
}
