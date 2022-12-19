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
    public class Get
    {
        public static Transaction<Response<byte>> FirmwareVersion = new Transaction<Response<byte>>(Action.GET_FIRMWARE_VERSION);
        public static Transaction<Response<int>> Time = new Transaction<Response<int>>(Action.GET_TIME);

        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public TValue* Values;
            public int Count;

            public object Recieve(xContent content)
            {
                Values = (TValue*)content.Data;
                Count = content.DataSize / sizeof(TValue);
                return this;
            }
        }
        public unsafe class Response : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public class Transaction<TResponse> : TransactionBase<TResponse> where TResponse : IResponseAdapter, new()
        {
            public virtual TransactionBase<TResponse> Prepare(ResponseHandle handle)
            {
                var transaction = new Transaction<TResponse>()
                {
                    name = name,
                    action = action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)action,
                    RequestId = transaction.Id
                };

                PacketBase.Add(request_data, request_header);
                PacketBase.Add(request_data, info);
                PacketBase.Add(request_data, end_packet);

                transaction.Data = request_data.ToArray();
                transaction.handle = handle;

                return transaction;
            }

            public Transaction(Action action) : base(action)
            {

            }

            public Transaction()
            {

            }
        }
    }
}
