using BrewGroup.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace BrewGroup.Communication.Transactions
{
    public class EventPacketBase<TResult> : ReceiverBase where TResult : IResponseAdapter, new()
    {
        protected static PacketHeaderT event_heder = PacketHeaderT.Init(PacketHeaderDescription.Event, DeviceInfo.Address);

        public Action Action { get; set; }

        public xEvent<TResult> EventReceive { get; set; }

        public TResult Result
        {
            get => (TResult)result;
            set => result = value;
        }

        public EventPacketBase(List<ReceiverBase> responses, Action action)
        {
            Action = action;
            responses?.Add(this);
        }

        public override unsafe bool Receive(xContent content)
        {
            PacketT* packet = (PacketT*)content.Data;
            PacketHeaderT header = event_heder;

            if (xConverter.Compare(&header, &packet->Header, sizeof(PacketHeaderT)) && (ushort)(object)Action == packet->Info.Action)
            {
                content.Data += sizeof(PacketT);
                content.DataSize -= sizeof(PacketT);

                Result = new TResult();
                Result.Recieve(content);

                EventReceive?.Invoke(this, Result);
                return true;
            }
            return false;
        }

        public EventPacketBase()
        {

        }
    }
}
