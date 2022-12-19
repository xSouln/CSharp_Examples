using Carousel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Carousel.Communication.Transactions
{
    public enum Action : ushort
    {
        GET_FIRMWARE_VERSION = 100,
        GET_STATUS,
        GET_POSITION,
        GET_OPTIONS,
        GET_MOTION_STATE,
        GET_CALIBRATION,

        SET = 1000,
        SET_POSITION,
        SET_OPTIONS,
        SET_CALIBRATION,
        SET_POD,

        TRY = 2000,
        TRY_STOP,
        TRY_RESET_POSITION,
        TRY_CALIBRATE,

        EVT = 10000,
        EVT_STATUS_CHANGED
    }

    public struct ResponseGetPositionT : IResponseAdapter
    {
        public float TotalAngle;
        public float RequestAngle;

        public int EncoderPosition;

        public unsafe object Recieve(xContent content)
        {
            this = *(ResponseGetPositionT*)content.Data;
            return this;
        }
    }

    public struct ResponseGetMotionStateT : IResponseAdapter
    {
        public StatusT Status;

        public float TotalAngle;
        public float RequestAngle;

        public float Power;

        public int MoveTime;

        public int EncoderPosition;

        public unsafe object Recieve(xContent content)
        {
            this = *(ResponseGetMotionStateT*)content.Data;
            return this;
        }
    }

    public unsafe class ResponseTryClearPosition : IResponseAdapter
    {
        public Result Result;
        public ResponseGetPositionT* Position;

        public object Recieve(xContent content)
        {
            Result = *(Result*)content.Data;
            content.Data += sizeof(Result);

            Position = (ResponseGetPositionT*)content.Data;
            return this;
        }
    }

    public unsafe class ResponseTryStop : IResponseAdapter
    {
        public Result Result;
        public StatusT* Status;

        public object Recieve(xContent content)
        {
            Result = *(Result*)content.Data;
            content.Data += sizeof(Result);

            Status = (StatusT*)content.Data;
            return this;
        }
    }

    public unsafe class ResponseSetPosition : IResponseAdapter
    {
        public Result Result;
        public ResponseGetPositionT* Position;

        public object Recieve(xContent content)
        {
            Result = *(Result*)content.Data;
            content.Data += sizeof(Result);

            Position = (ResponseGetPositionT*)content.Data;
            content.Data += sizeof(ResponseGetPositionT);
            return this;
        }
    }
}
