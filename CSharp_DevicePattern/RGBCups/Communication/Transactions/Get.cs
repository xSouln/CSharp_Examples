using CupsControl.Types;
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
    public class Get
    {
        public static Transaction<ResponseResult<ColorT>, RequestGetPixelsT> Pixels = new Transaction<ResponseResult<ColorT>, RequestGetPixelsT>(Action.GET_PIXELS);
        public static Transaction<ResponseGetStatus> Status = new Transaction<ResponseGetStatus>(Action.GET_STATUS);

        public unsafe class ResponseResult<TValue> : IResponseAdapter where TValue : unmanaged
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

        public unsafe class ResponseResult : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public class Transaction<TResult> : TransactionBase<TResult> where TResult : IResponseAdapter, new()
        {
            public virtual TransactionBase<TResult> Prepare(ResponseHandle handle)
            {
                var transaction = new Transaction<TResult>()
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

        public class Transaction<TResult, TRequest> : TransactionBase<TResult> where TResult : IResponseAdapter, new() where TRequest : unmanaged
        {
            public unsafe virtual TransactionBase<TResult> Prepare(ResponseHandle handle, TRequest request)
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
                    RequestId = transaction.Id,
                    ContentSize = (ushort)sizeof(TRequest)
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
