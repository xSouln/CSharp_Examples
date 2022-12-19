using CupsControl.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;
using static CupsControl.Types.RequestSetPixels;

namespace CupsControl.Communication.Transactions
{
    public class Set
    {
        public static TransactionSetPixels Pixels = new TransactionSetPixels(Action.SET_PIXELS);//RequestSetColorT
        public static Transaction<Response, RequestSetColorT> Color = new Transaction<Response, RequestSetColorT>(Action.SET_COLOR);//RequestSetTemplateByIdT
        public static Transaction<Response, RequestSetTemplateByIdT> TemplateById = new Transaction<Response, RequestSetTemplateByIdT>(Action.SET_TEMPLATE_BY_ID);

        public unsafe class Response : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
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

        public class TransactionSetPixels : TransactionBase<Response>
        {
            public unsafe virtual TransactionSetPixels Prepare(ResponseHandle handle, RequestSetPixelsT request, ColorT[] pixels)
            {
                var transaction = new TransactionSetPixels()
                {
                    name = name,
                    action = action,
                    EventResponseReceive = EventResponseReceive
                };

                List<byte> request_data = new List<byte>();

                PacketInfoT info = new PacketInfoT
                {
                    Action = (ushort)(object)action,
                    ContentSize = (ushort)(sizeof(RequestSetPixelsT) + (sizeof(ColorT) * pixels.Length)),
                    RequestId = transaction.Id
                };

                PacketBase.Add(request_data, request_header);
                PacketBase.Add(request_data, info);
                PacketBase.Add(request_data, request);
                PacketBase.Add(request_data, pixels);
                PacketBase.Add(request_data, end_packet);

                transaction.Data = request_data.ToArray();
                transaction.handle = handle;

                return transaction;
            }

            public TransactionSetPixels(Action action) : base(action)
            {

            }

            public TransactionSetPixels()
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
