using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Device.Communication
{
    public partial class Requests : ModelBase
    {
        public ResponseHandle Handle = new ResponseHandle();
        public List<ReceiverBase> List = new List<ReceiverBase>();

        public Requests(Control control)
        {
            Parent = control;
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
