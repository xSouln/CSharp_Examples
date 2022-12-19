using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;
using BrewGroup.Types;

namespace BrewGroup.Communication.Transactions
{
    public class Set
    {
        public static Transaction<Response<OptionsT>, OptionsT> Options = new Transaction<Response<OptionsT>, OptionsT>(Action.SET_OPTIONS);
        public static Transaction<ResponseSetPosition, RequestSetPositionT> Position = new Transaction<ResponseSetPosition, RequestSetPositionT>(Action.SET_POSITION);
        public static Transaction<Response, CalibrationT> Calibration = new Transaction<Response, CalibrationT>(Action.SET_CALIBRATION);

        public unsafe class Response : IResponseAdapter
        {
            public Result Result;

            public object Recieve(xContent content)
            {
                Result = *(Result*)content.Data;
                return this;
            }
        }

        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public Result Result;
            public TValue* Values;
            public int Count;

            public object Recieve(xContent content)
            {
                Result = *(Result*)content.Data;
                content.Data += sizeof(Result);

                Values = (TValue*)content.Data;
                Count = content.DataSize / sizeof(TValue);
                return this;
            }
        }

        public class Transaction<TResponse, TRequest> : TransactionBase<TResponse> where TResponse : IResponseAdapter, new() where TRequest : IRequestAdapter
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
                    ContentSize = (ushort)request.GetSize(),
                    RequestId = transaction.Id
                };

                PacketBase.Add(request_data, request_header);
                PacketBase.Add(request_data, info);
                PacketBase.Add(request_data, request.GetData());
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
