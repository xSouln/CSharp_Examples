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
    public class Try
    {
        public static Transaction<Response<StatusT>> Stop = new Transaction<Response<StatusT>>(Action.TRY_STOP);
        public static Transaction<Response<StatusT>> Open = new Transaction<Response<StatusT>>(Action.TRY_OPEN);
        public static Transaction<Response<StatusT>> Close = new Transaction<Response<StatusT>>(Action.TRY_CLOSE);
        public static Transaction<Response<StatusT>, RequestTrySetPositionT> SetPosition = new Transaction<Response<StatusT>, RequestTrySetPositionT>(Action.TRY_SET_POSITION);
        public static Transaction<Response<StatusT>, RequestTryDropPodT> DropePode = new Transaction<Response<StatusT>, RequestTryDropPodT>(Action.TRY_DROP_POD);

        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public Result Result;
            public TValue* Value;

            public object Recieve(xContent content)
            {
                Result = *(Result*)content.Data;
                content.Data += sizeof(Result);

                Value = (TValue*)content.Data;

                return this;
            }
        }

        public class Transaction<TResponse, TRequest> : TransactionBase<TResponse> where TResponse : IResponseAdapter, new() where TRequest : unmanaged
        {
            public unsafe virtual Transaction<TResponse, TRequest> Prepare(ResponseHandle handle, TRequest request)
            {
                var transaction = new Transaction<TResponse, TRequest>()
                {
                    name = name,
                    Action = Action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)Action,
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

        public class Transaction<TResponse> : TransactionBase<TResponse> where TResponse : IResponseAdapter, new()
        {
            public virtual Transaction<TResponse> Prepare(ResponseHandle handle)
            {
                var transaction = new Transaction<TResponse>()
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
