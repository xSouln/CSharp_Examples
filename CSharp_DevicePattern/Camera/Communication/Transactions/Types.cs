using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.Communication.Transactions
{
    public enum Action : ushort
    {
		GET_FIRMWARE_VERSION = 100,
		GET_SNAPSHOT_RGB565,
		GET_SNAPSHOT_JPEG,
		GET_OPTIONS,
		GET_STATUS,

		SET = 1000,
		SET_OPTIONS,
		SET_OUTPUT_FORMAT,
		SET_RESOLUTION,
		SET_SATURATION,
		SET_CONTRAST,
		SET_BRIGHTNESS,
		SET_LIGHTMODE,
		SET_SPECIALEFFECT,
		SET_QUANTIZATION,
		SET_AGC_GAIN,
		SET_CONFIGURATION,

		TRY = 2000,

		EVT = 10000,

		EVT_RGB565_TRANSFER_START,
		EVT_RGB565_TRANSFER,
		EVT_RGB565_TRANSFER_END,

		EVT_JPEG_TRANSFER_START,
		EVT_JPEG_TRANSFER,
		EVT_JPEG_TRANSFER_END,
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
