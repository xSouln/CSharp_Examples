using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLib.Common;

namespace xLib.Transceiver
{
    public class TransmitterBase
    {
        protected xAction<bool, byte[]> transmitter;
        protected int try_count = 1;
        protected int try_number = 0;
        protected int response_time_out = 100;
        protected int response_time = 0;
        protected volatile TransmissionStatus status;
        protected AutoResetEvent transmition_synchronize = new AutoResetEvent(true);
        protected IReceiver receiver;
        protected ResponseHandle handle;
        protected string name;
        protected byte[] data;

        public xEvent<TransmissionStatus> EventTimeOut;

        public virtual xAction<string> Tracer { get; set; }

        public virtual ResponseHandle Handle
        {
            get => handle;
            set => handle = value;
        }

        public virtual byte[] Data
        {
            get => data;
            set => data = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int ResponseTimeOut => response_time_out;

        public int ResponseTime => response_time;

        public IReceiver GetReceiver() => receiver;

        public int TryCount
        {
            get => try_count;
            set
            {
                if (try_count > 0) { try_count = value; }
            }
        }

        public int TryNumber => try_number;

        public xAction<bool, byte[]> Transmitter
        {
            get => transmitter;
            set => transmitter = value;
        }

        public TransmissionStatus Status => status;

        public void Accept()
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (status == TransmissionStatus.IsTransmit)
                {
                    status = TransmissionStatus.Complite;
                }
            }
            finally
            {
                transmition_synchronize.Set();
            }
        }

        protected virtual void TransmitAction()
        {
            if (status == TransmissionStatus.IsTransmit)
            {
                if (try_number < try_count)
                {
                    if (transmitter == null || !transmitter(Data))
                    {
                        status = TransmissionStatus.ErrorTransmite;
                        Tracer?.Invoke("Transmit: " + Name + " " + status);
                        return;
                    }
                    try_number++;
                    Tracer?.Invoke("Transmit: " + Name + " try: " + try_number);
                }
                else
                {
                    status = TransmissionStatus.TimeOut;
                    Tracer?.Invoke("TimeOut: " + Name);
                    EventTimeOut?.Invoke(this, TransmissionStatus.TimeOut);
                }
            }
        }

        protected virtual TransmitterBase Transmition()
        {
            if (status != TransmissionStatus.Free)
            {
                return this;
            }

            status = TransmissionStatus.Prepare;

            if (!(bool)Handle?.Add(this))
            {
                status = TransmissionStatus.Busy;
                return this;
            }
            else
            {
                status = TransmissionStatus.IsTransmit;
            }

            try_number = 0;
            response_time = 0;

            Stopwatch time_transmition = new Stopwatch();
            Stopwatch time_transmit_action = new Stopwatch();

            time_transmition.Start();
            do
            {
                TransmitAction();
                time_transmit_action.Restart();
                while (status == TransmissionStatus.IsTransmit && time_transmit_action.ElapsedMilliseconds < response_time_out)
                {
                    Thread.Sleep(1);
                }
            }
            while (status == TransmissionStatus.IsTransmit);

            time_transmition.Stop();
            time_transmit_action.Stop();
            response_time = (int)time_transmition.ElapsedMilliseconds;
            return this;
        }

        protected virtual async Task<TransmitterBase> TransmitionAsync()
        {
            try
            {
                transmition_synchronize.WaitOne();

                if (status != TransmissionStatus.Free) { return this; }
                status = TransmissionStatus.Prepare;

                if (!(bool)Handle?.Add(this))
                {
                    status = TransmissionStatus.Busy;
                    return this;
                }
                else
                {
                    status = TransmissionStatus.IsTransmit;
                }
            }
            finally
            {
                transmition_synchronize.Set();
            }

            try_number = 0;
            response_time = 0;

            Stopwatch time_transmition = new Stopwatch();
            Stopwatch time_transmit_action = new Stopwatch();

            time_transmition.Start();
            do
            {
                TransmitAction();
                time_transmit_action.Restart();
                while (status == TransmissionStatus.IsTransmit && time_transmit_action.ElapsedMilliseconds < response_time_out)
                {
                    await Task.Delay(1);
                }
            }
            while (status == TransmissionStatus.IsTransmit);

            time_transmition.Stop();
            time_transmit_action.Stop();
            response_time = (int)time_transmition.ElapsedMilliseconds;
            return this;
        }

        public virtual void Break()
        {
            transmition_synchronize.WaitOne();
            status = TransmissionStatus.Free;
            transmition_synchronize.Set();

            Handle?.Remove(this);
        }

        public virtual TransmitterBase Transmition(xAction<bool, byte[]> transmitter, int try_count, int response_time_out)
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || status != TransmissionStatus.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time_out;
            this.try_number = 0;

            var result = Transmition();
            return result;
        }

        public static TRequest Transmition<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time_out) where TRequest : TransmitterBase
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || request.status != TransmissionStatus.Free) { return null; }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time_out;
            request.try_number = 0;

            var result = request.Transmition();
            return (TRequest)result;
        }

        public virtual async Task<TransmitterBase> TransmitionAsync(xAction<bool, byte[]> transmitter, int try_count, int response_time_out)
        {
            if (transmitter == null || try_count <= 0 || response_time_out <= 0 || status != TransmissionStatus.Free) { return null; }
            this.transmitter = transmitter;
            this.try_count = try_count;
            this.response_time_out = response_time_out;
            this.try_number = 0;

            //var result = await Task.Run(() => Transmition());
            var result = await Task.Run(() => TransmitionAsync());
            return result;
        }

        public static async Task<TRequest> TransmitionAsync<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time, CancellationTokenSource cancellation) where TRequest : TransmitterBase
        {
            if (transmitter == null || try_count <= 0 || response_time <= 0 || request.status != TransmissionStatus.Free) { return null; }

            if (cancellation == null) { cancellation = new CancellationTokenSource(); }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time;
            request.try_number = 0;

            var result = await Task.Run(() => request.Transmition(), cancellation.Token);
            return (TRequest)result;
        }

        public static async Task<TRequest> TransmitionAsync<TRequest>(TRequest request, xAction<bool, byte[]> transmitter, int try_count, int response_time) where TRequest : TransmitterBase
        {
            if (transmitter == null || try_count <= 0 || response_time <= 0 || request.status != TransmissionStatus.Free) { return null; }

            request.transmitter = transmitter;
            request.try_count = try_count;
            request.response_time_out = response_time;
            request.try_number = 0;

            var result = await Task.Run(() => request.Transmition());
            return (TRequest)result;
        }

        public static void Trace(string context, TransmitterBase request)
        {
            if (request != null)
            {
                xTracer.Message(request.Name + " " + request.Status + ", response time: " + request.ResponseTime);
            }
            else
            {
                xTracer.Message("" + context + " request = null");
            }
        }

        public static void Trace(string context, string name, TransmitterBase request)
        {
            if (request != null)
            {
                xTracer.Message(name + " => response: \"" + request.Name + "\"; result: " + request.Status + "; response time: " + request.ResponseTime);
            }
            else
            {
                xTracer.Message("" + context + " request = null");
            }
        }
    }
}