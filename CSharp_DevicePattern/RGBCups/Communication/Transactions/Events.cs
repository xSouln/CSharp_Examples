using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace CupsControl.Communication.Transactions
{
    public class Events
    {
        public static List<ReceiverBase> List = new List<ReceiverBase>();

        public unsafe class EventResult<TResponse> : IResponseAdapter where TResponse : unmanaged
        {
            public TResponse Result;

            public object Recieve(xContent content)
            {
                Result = *(TResponse*)content.Data;
                return this;
            }
        }
    }
}
