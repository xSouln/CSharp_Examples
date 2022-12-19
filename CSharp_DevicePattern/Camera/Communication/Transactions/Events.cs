using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Camera.Communication.Transactions
{
    public class Events
    {
        public static List<ReceiverBase> List = new List<ReceiverBase>();

        public static EventPacketBase<EventResult<ActionResult>> SnapshotRGB565_TransferStart = new EventPacketBase<EventResult<ActionResult>>(List, Action.EVT_RGB565_TRANSFER_START);
        public static EventPacketBase<EventSnapshot> SnapshotRGB565_TransferSegment = new EventPacketBase<EventSnapshot>(List, Action.EVT_RGB565_TRANSFER);
        public static EventPacketBase<EventResult<ActionResult>> SnapshotRGB565_TransferEnd = new EventPacketBase<EventResult<ActionResult>>(List, Action.EVT_RGB565_TRANSFER_END);

        public static EventPacketBase<EventResult<ActionResult>> SnapshotJPEG_TransferStart = new EventPacketBase<EventResult<ActionResult>>(List, Action.EVT_JPEG_TRANSFER_START);
        public static EventPacketBase<EventJPEG> SnapshotJPEG_TransferSegment = new EventPacketBase<EventJPEG>(List, Action.EVT_JPEG_TRANSFER);
        public static EventPacketBase<EventResult<ActionResult>> SnapshotJPEG_TransferEnd = new EventPacketBase<EventResult<ActionResult>>(List, Action.EVT_JPEG_TRANSFER_END);

        public unsafe class EventResult<TResponse> : IResponseAdapter where TResponse : unmanaged
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
