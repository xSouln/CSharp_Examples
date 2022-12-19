using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI;

namespace xLib.Common
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected object parent;
        protected object control;
        protected string name;
        protected ViewModelBase view_model;

        public virtual string Name
        {
            get => name;
            set => name = value;
        }

        public object GetParent() => parent;
        public object GetControl() => control;
        public ViewModelBase GetViewModel => view_model;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Parent
        {
            get => parent;
            set => parent = value;
        }

        public virtual void Dispose()
        {
            if (view_model != null)
            {
                view_model.Dispose();
                view_model = null;
            }
        }
    }

    public class ModelBase<TControl, TViewModel> : ModelBase where TControl : class where TViewModel : ViewModelBase
    {
        public TControl Control
        {
            get => control != null ? (TControl)control : null;
            set => control = value;
        }

        public TViewModel ViewModel
        {
            get => view_model != null ? (TViewModel)view_model : null;
            set => view_model = value;
        }

        public ModelBase(TControl control)
        {
            Control = control;
        }
    }
}
