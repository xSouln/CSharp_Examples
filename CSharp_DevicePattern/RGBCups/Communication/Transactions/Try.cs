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
    public class Try
    {
        //public static Transaction<ResponseResult<ActionResult, int>> ResetTime = new Transaction<ResponseResult<ActionResult, int>>(Action.TRY_RESET_TIME);

        public unsafe class ResponseResult<TResult> : IResponseAdapter where TResult : unmanaged
        {
            public TResult Result;

            public object Recieve(xContent content)
            {
                Result = *(TResult*)content.Data;
                return this;
            }
        }

        public unsafe class ResponseResult<TResult, TValue> : IResponseAdapter where TResult : unmanaged where TValue : unmanaged
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
                    action = action
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)action,
                    ContentSize = (ushort)sizeof(TValue)
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

        public class Transaction<TResult> : TransactionBase<TResult> where TResult : IResponseAdapter, new()
        {
            public virtual Transaction<TResult> Prepare(ResponseHandle handle)
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
                    RequestId = transaction.Id,
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
