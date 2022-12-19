using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Communication.Transactions
{
    public enum Action : ushort
    {
        GET_FIRMWARE_VERSION = 100,
        GET_TIME,

        SET = 1000,
        SET_TIME,

        TRY = 2000,
        TRY_RESET_TIME,

        EVT = 10000,
    }

    public enum ActionResult : ushort
    {
        ACCEPT = 0,
        ERROR_DATA,
        ERROR_CONTENT_SIZE,
        ERROR_REQUEST,
        ERROR_RESOLUTION,
        UNKNOWN_COMMAND,
        BUSY,
        OUTSIDE,
        ERROR_ACTION
    }
}
