using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Interfaces
{
    public enum EventSelector
    {
        Idle
    }

    public interface IEventSelector
    {
        void Eventlistener(object context, EventSelector selector, object arg);
    }
}
