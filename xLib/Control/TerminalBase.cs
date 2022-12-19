using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.UI;

namespace xLib.Controls
{
    public abstract class TerminalBase : ITerminal
    {
        public string Name { get; set; } = "TerminalBase";
        public List<DeviceBase> Devices { get; set; }

        protected object parent;
        protected ViewModelBase view_model;

        public TerminalBase()
        {
            Devices = new List<DeviceBase>();
        }

        public virtual void AddDevice(DeviceBase device)
        {
            Devices.Add(device);
        }

        public virtual xAction<bool, byte[]> GetTransmitter()
        {
            return null;
        }

        public ViewModelBase GetViewModel() => view_model;
        public object GetParent => parent;

        public virtual void Dispose()
        {
            if (Devices != null)
            {
                foreach (IDevice device in Devices)
                {
                    device?.Dispose();
                }

                Devices.Clear();
            }

            if (view_model != null)
            {
                view_model.Dispose();
                view_model = null;
            }
        }
    }

    public abstract class TerminalBase<TViewModel> : TerminalBase where TViewModel : ViewModelBase
    {
        public TViewModel ViewModel
        {
            get => view_model != null ? (TViewModel)view_model : null;
            set => view_model = value;
        }
    }
}
