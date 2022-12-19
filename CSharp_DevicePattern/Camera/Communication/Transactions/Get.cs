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
    public class Get
    {
        public static Transaction<ResponseResult<byte>> FirmwareVersion = new Transaction<ResponseResult<byte>>(Action.GET_FIRMWARE_VERSION);

        public static Transaction<ResponseResult> SnaphotRGB565 = new Transaction<ResponseResult>(Action.GET_SNAPSHOT_RGB565);
        public static Transaction<ResponseResult> SnaphotJPEG = new Transaction<ResponseResult>(Action.GET_SNAPSHOT_JPEG);
        public static Transaction<ResponseResult<OptionsT>> Options = new Transaction<ResponseResult<OptionsT>>(Action.GET_OPTIONS);

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

        public unsafe class ResponseResult<TActionResult, TValue> : IResponseAdapter where TValue : unmanaged where TActionResult : unmanaged
        {
            public TActionResult ActionResult;
            public TValue* Values;
            public int Count;

            public object Recieve(xContent content)
            {
                ActionResult = *(TActionResult*)content.Data;

                content.Data += sizeof(TActionResult);
                content.DataSize -= sizeof(TActionResult);

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

        public class Transaction<TResult, TRequest> : TransactionBase<TResult> where TResult : IResponseAdapter, new() where TRequest : unmanaged
        {
            public virtual unsafe Transaction<TResult, TRequest> Prepare(ResponseHandle handle, TRequest request)
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
                this.handle = handle;

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
