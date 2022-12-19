using Slider.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Slider.Communication.Transactions
{
    public enum Action : ushort
    {
        GET_FIRMWARE_VERSION = 100,
        GET_STATUS,
        GET_OPTIONS,

        SET = 1000,
        SET_OPTIONS,

        TRY = 2000,
        TRY_OPEN,
        TRY_CLOSE,
        TRY_STOP,
        TRY_DROP_POD,
        TRY_SET_POSITION,

        EVT = 10000,
        EVT_OPEN,
        EVT_CLOSE,
        EVT_OVERCURRENT,
        EVT_STATUS_CHANGED
    }

    public struct RequestTrySetPositionT : IRequestAdapter
    {
        public float Power;
        public int TimeOut;
        public MoveMode Mode;
        public MovePosition Position;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestTrySetPositionT);
        }
    }

    public struct RequestTryDropPodT : IRequestAdapter
    {
        public int OpenTime;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestTryDropPodT);
        }
    }
}
