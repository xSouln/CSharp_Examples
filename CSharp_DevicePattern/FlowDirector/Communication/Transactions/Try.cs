using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace FlowDirector.Communication.Transactions
{
    public class Try
    {
        public static Transaction<ResponseTryClearPosition> ResetPosition = new Transaction<ResponseTryClearPosition>(Action.TRY_RESET_POSITION);
        public static Transaction<ResponseTryStop> Stop = new Transaction<ResponseTryStop>(Action.TRY_STOP);

        public unsafe class Response<TResult> : IResponseAdapter where TResult : unmanaged
        {
            public TResult Result;

            public object Recieve(xContent content)
            {
                Result = *(TResult*)content.Data;
                return this;
            }
        }

        public unsafe class Response<TResult, TValue> : IResponseAdapter where TResult : unmanaged where TValue : unmanaged
        {
            public TResult Result;
            public TValue* Values;
            public int Count;

            public object Recieve(xContent content)
            {
                Result = *(TResult*)content.Data;

                Values = (TValue*)(content.Data + sizeof(TResult));
                Count = content.DataSize / sizeof(TValue);

                return this;
            }
        }

        public class Transaction<TResult, TValue> : TransactionBase<TResult> where TResult : IResponseAdapter, new() where TValue : unmanaged
        {
            public unsafe virtual Transaction<TResult, TValue> Prepare(ResponseHandle handle, TValue request)
            {
                var transaction = new Transaction<TResult, TValue>()
                {
                    name = name,
                    Action = Action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)Action,
                    ContentSize = (ushort)sizeof(TValue),
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
        }

        public class Transaction<TResult> : TransactionBase<TResult> where TResult : IResponseAdapter, new()
        {
            public virtual Transaction<TResult> Prepare(ResponseHandle handle)
            {
                var transaction = new Transaction<TResult>()
                {
                    name = name,
                    Action = Action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)Action,
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
