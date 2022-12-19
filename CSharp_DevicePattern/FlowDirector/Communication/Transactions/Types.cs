using FlowDirector.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace FlowDirector.Communication.Transactions
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

        TRY = 2000,
        TRY_STOP,
        TRY_RESET_POSITION,

        EVT = 10000,
    }

    public struct ResponseGetPositionT : IResponseAdapter
    {
        public float TotalAngle;
        public float RequestAngle;

        public int StepPosition;

        public unsafe object Recieve(xContent content)
        {
            this = *(ResponseGetPositionT*)content.Data;
            return this;
        }
    }

    public struct RequestSetPositionT : IRequestAdapter
    {
        public float Angle;
        public float Speed;

        public int Time;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestSetPositionT);
        }
    }

    public struct ResponseGetMotionStateT : IResponseAdapter
    {
        public StatusT Status;

        public float TotalAngle;
        public float RequestAngle;

        public float Speed;

        public int MoveTime;

        public int StepPosition;

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
