using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI;

namespace xLib.Transceiver
{
    public abstract class ReceiverBase : INotifyPropertyChanged, IReceiver
    {
        protected string name = "";
        protected IResponseAdapter result;
        public xEvent<string> Tracer;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IResponseAdapter GetResult() => result;

        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual bool Receive(xContent content)
        {
            return false;
        }
    }
}
