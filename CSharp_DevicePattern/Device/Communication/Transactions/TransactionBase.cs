using Device.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Device.Communication.Transactions
{
    public class TransactionBase<TResult> : TransmitterBase, IReceiver where TResult : IResponseAdapter, new()
    {
        public xEvent<TResult> EventResponseReceive { get; set; }

        protected static PacketHeaderT request_header = PacketHeaderT.Init(PacketHeaderDescription.Request, DeviceInfo.Address);
        protected static PacketHeaderT response_header = PacketHeaderT.Init(PacketHeaderDescription.Response, DeviceInfo.Address);
        protected const string end_packet = "\r";

        protected Action action;
        protected TResult result;
        public uint Id { get; set; }

        public TransactionBase(Action action)
        {
            receiver = this;
            Name = "" + action;
            this.action = action;
        }

        public TransactionBase()
        {
            receiver = this;
            Id = (uint)new Random().Next();
        }

        public unsafe bool Receive(xContent content)
        {
            PacketT* packet = (PacketT*)content.Data;
            PacketHeaderT header = response_header;

            if (//xConverter.Compare(&header, &packet->Header, sizeof(PacketHeaderT))
                packet->Header.Identificator == response_header.Identificator
                && packet->Header.DeviceKey == response_header.DeviceKey
                && (packet->Info.Action == (ushort)(object)action)
                && packet->Info.RequestId == Id)
            {
                content.Data += sizeof(PacketT);
                content.DataSize -= sizeof(PacketT);

                result = new TResult();
                result.Recieve(content);

                EventResponseReceive?.Invoke(this, result);
                return true;
            }
            return false;
        }

        public TResult Result => result;

        public IResponseAdapter GetResult() => result;
    }
}
