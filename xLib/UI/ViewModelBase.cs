using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xLib.UI
{
    public abstract class ViewModelBase : UINotifyPropertyChanged
    {
        protected string name;
        protected object control;
        protected object parent;
        protected UIElement element;

        public virtual UIElement UIModel
        {
            get => element;
            set
            {
                if (element != value)
                {
                    element = value;
                    OnPropertyChanged(nameof(UIModel));
                }
            }
        }

        public virtual string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public virtual void Update()
        {

        }

        public virtual ViewModelBase Clone()
        {
            return null;
        }

        public virtual object GetParent() => parent;

        public virtual object GetControl() => control;

        public virtual void Dispose()
        {

        }
    }

    public class ViewModelBase<TControl> : ViewModelBase where TControl : class
    {
        public virtual TControl Control
        {
            get => control != null ? (TControl)control : null;
            set => control = value;
        }

        public object Parent
        {
            get => parent;
            set => parent = value;
        }

        public ViewModelBase(TControl control)
        {
            Control = control;
        }
    }

    public class ViewModelBase<TControl, TParent> : ViewModelBase where TControl : class where TParent : class
    {
        public virtual TControl Control
        {
            get => control != null ? (TControl)control : null;
            set => control = value;
        }

        public virtual TParent Parent
        {
            get => parent != null ? (TParent)parent : null;
            set => parent = value;
        }

        public ViewModelBase(TControl control, TParent parent)
        {
            Control = control;
            Parent = parent;
        }
    }
}
