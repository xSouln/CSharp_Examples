using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public enum TransmissionStatus
    {
        Free,
        Prepare,
        IsTransmit,
        Complite,
        TimeOut,
        ErrorTransmite,
        ErrorTransmiteAction,
        Busy,
        AddError
    }

    public class ResponseHandle
    {
        protected List<TransmitterBase> requests = new List<TransmitterBase>();
        protected AutoResetEvent read_write_synchronizer = new AutoResetEvent(true);
        protected Semaphore queue_size;
        protected Thread thread;

        public ResponseHandle(int line_size)
        {
            if (line_size < 1)
            {
                line_size = 10;
            }
            queue_size = new Semaphore(line_size, line_size);
        }

        public ResponseHandle()
        {
            queue_size = new Semaphore(10, 10);
            //thread = new Thread(thread_handler);
            //thread.Start();
        }

        protected virtual void Update()
        {
            int i = 0;
            while (i < requests.Count)
            {
                switch (requests[i].Status)
                {
                    case TransmissionStatus.Complite:
                        requests[i].Accept();
                        requests.RemoveAt(i);
                        break;

                    case TransmissionStatus.IsTransmit:
                        i++;
                        break;

                    case TransmissionStatus.Prepare:
                        i++;
                        break;

                    default: requests.RemoveAt(i);
                        break;
                }
            }
        }

        public virtual bool Add(TransmitterBase request)
        {
            try
            {
                read_write_synchronizer.WaitOne();

                Update();
                if (requests.Count >= 20)
                {
                    return false;
                }
                requests.Add(request);
            }
            finally
            {
                read_write_synchronizer.Set();
            }
            return true;
        }

        public virtual void Remove(TransmitterBase request)
        {
            try
            {
                read_write_synchronizer.WaitOne();

                for (int i = 0; i < this.requests.Count; i++)
                {
                    if (this.requests[i] == request)
                    {
                        requests.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            finally
            {
                read_write_synchronizer.Set();
            }
        }

        public virtual bool Receive(xContent content)
        {
            bool result = false;
            try
            {
                read_write_synchronizer.WaitOne();

                Update();
                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i].GetReceiver().Receive(content);
                    if (result)
                    {
                        requests[i].Accept();
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                read_write_synchronizer.Set();
            }
            return result;
        }

        public virtual bool Accept(TransmitterBase request)
        {
            bool result = false;
            try
            {
                read_write_synchronizer.WaitOne();

                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i] == request;
                    if (result)
                    {
                        requests[i].Accept();
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                read_write_synchronizer.Set();
            }
            return result;
        }
    }
}
