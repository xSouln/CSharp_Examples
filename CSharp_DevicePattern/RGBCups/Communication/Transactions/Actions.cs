using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CupsControl.Communication.Transactions
{
    public enum Action : ushort
    {
        GET_FIRMWARE_VERSION = 100,
        GET_PIXELS,
        GET_STATUS,
        GET_TEMPLATE_ID,

        SET = 1000,
        SET_PIXELS,
        SET_PIXELS_STATE,
        SET_COLOR,
        SET_TEMPLATE,
        SET_TEMPLATE_BY_ID,

        TRY = 2000,
        TRY_DRAWING_START,
        TRY_DRAWING_STOP,

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
