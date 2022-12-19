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
    public class Set
    {
        public static Transaction<ResponseResult<int>, int> Time = new Transaction<ResponseResult<int>, int>(Action.SET_TIME);

        public unsafe class ResponseResult : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public unsafe class ResponseResult<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public ActionResult Result;
            public TValue* Values;
            public int Count;

            public object Recieve(xContent content)
            {
                Result = *(ActionResult*)content.Data;
                content.Data += sizeof(ActionResult);

                Values = (TValue*)content.Data;
                Count = content.DataSize / sizeof(TValue);
                return this;
            }
        }

        public class Transaction<TResult, TRequest> : TransactionBase<TResult> where TResult : IResponseAdapter, new() where TRequest : unmanaged
        {
            public unsafe virtual Transaction<TResult, TRequest> Prepare(ResponseHandle handle, TRequest request)
            {
                var transaction = new Transaction<TResult, TRequest>()
                {
                    name = name,
                    action = action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)action,
                    ContentSize = (ushort)sizeof(TRequest),
                    RequestId = transaction.Id
                };

                PacketBase.Add(request_data, request_header);
                PacketBase.Add(request_data, info);
                PacketBase.Add(request_data, request);
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
