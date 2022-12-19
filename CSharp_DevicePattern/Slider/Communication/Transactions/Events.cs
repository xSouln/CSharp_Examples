using Slider.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Slider.Communication.Transactions
{
    public class Events
    {
        public static List<ReceiverBase> List = new List<ReceiverBase>();

        public static EventPacketBase<StatusT> Open = new EventPacketBase<StatusT>(List, Action.EVT_OPEN);
        public static EventPacketBase<StatusT> Close = new EventPacketBase<StatusT>(List, Action.EVT_CLOSE);
        public static EventPacketBase<StatusT> Overcurrent = new EventPacketBase<StatusT>(List, Action.EVT_OVERCURRENT);
        public static EventPacketBase<StatusT> StatusChanged = new EventPacketBase<StatusT>(List, Action.EVT_STATUS_CHANGED);

        public unsafe class Event<TResponse> : IResponseAdapter where TResponse : unmanaged
        {
            public TResponse Result;

            public object Recieve(xContent content)
            {
                Result = *(TResponse*)content.Data;
                return this;
            }
        }

        public unsafe class EventSnapshot : IResponseAdapter
        {
            public uint* Pixels;
            public int PixelsCount;

            public object Recieve(xContent content)
            {
                Pixels = (uint*)content.Data;
                PixelsCount = content.DataSize / sizeof(uint);

                return this;
            }
        }

        public unsafe class EventJPEG : IResponseAdapter
        {
            public byte* Data;
            public int DataSize;

            public object Recieve(xContent content)
            {
                Data = content.Data;
                DataSize = content.DataSize;

                return this;
            }
        }
    }
}
