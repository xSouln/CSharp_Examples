using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Transceiver;

namespace Carousel.Communication
{
    public partial class Responses
    {
        public List<ReceiverBase> List;
        public Control Parent { get; set; }

        public Responses(Control parent)
        {
            Parent = parent;

            List = new List<ReceiverBase>();
        }

        public unsafe bool Identification(xContent content)
        {
            foreach (ReceiverBase response in Transactions.Events.List)
            {
                if (response.Receive(content))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
