using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;
using Camera.Types;

namespace Camera.Communication.Transactions
{
    public class Set
    {
        public static Transaction<ResponseResult<OptionsT>, OptionsT> Options = new Transaction<ResponseResult<OptionsT>, OptionsT>(Action.SET_OPTIONS);
        public static TransactionData<ResponseResult> Configuration = new TransactionData<ResponseResult>(Action.SET_CONFIGURATION);

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

        public class TransactionData<TResult> : TransactionBase<TResult> where TResult : IResponseAdapter, new()
        {
            public unsafe virtual TransactionData<TResult> Prepare(ResponseHandle handle, byte[] request)
            {
                var transaction = new TransactionData<TResult>()
                {
                    name = name,
                    Action = Action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)Action,
                    ContentSize = (ushort)request.Length,
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

            public TransactionData(Action action) : base(action)
            {

            }

            public TransactionData()
            {

            }
        }

        public class Transaction<TResult, TRequest> : TransactionBase<TResult> where TResult : IResponseAdapter, new() where TRequest : unmanaged
        {
            public unsafe virtual Transaction<TResult, TRequest> Prepare(ResponseHandle handle, TRequest request)
            {
                var transaction = new Transaction<TResult, TRequest>()
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
    }
}
