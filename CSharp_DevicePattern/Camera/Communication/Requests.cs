using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Camera.Communication
{
    public partial class Requests
    {
        public ResponseHandle Handle = new ResponseHandle();
        public List<ReceiverBase> List = new List<ReceiverBase>();

        public Control Parent { get; set; }

        public Requests(Control device)
        {
            Parent = device;
        }

        public bool Identification(xContent content)
        {
            /*
            foreach (xResponseBase response in List)
            {
                if (response.Identification(content))
                {
                    return true;
                }
            }
            */
            return Handle.Receive(content);
        }
    }
}
