using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Carousel.Types
{
    public class DeviceInfo
    {
        public const uint Address = 0x79533567;
    }

    public interface IRequestAdapter
    {
        int GetSize();
        byte[] GetData();
    }

    public enum Result : short
    {
        Accept,
        Error,
        InvalidParameter,
        Busy,
        TimeOut,
        NotSupported,
        ValueIsNotFound,
        RequestIsNotFound,
        LinkError,
        ComponentInitializationError,
    }

    public struct OptionsT : IRequestAdapter, IResponseAdapter
    {
        public float Acceleration;
        public float Deceleration;

        public float StartSpeed;
        public float StopSpeed;

        public float Power;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(OptionsT);
        }

        public unsafe object Recieve(xContent content)
        {
            this = *(OptionsT*)content.Data;

            return this;
        }
    }

    public enum MotionState
    {
        Stopped,
        MovingForward,
        MovingBackward,
    }

    public enum MotionError
    {
        NoError,
        Timeout,
        Overcurrent,
    }

    public enum CalibrationState
    {
        Idle,
        FindZeroMark,
        MoveOutAtZeroMarkAndResetSteps,
        FindZeroMarkBackSide,
        MoveOutAtZeroMarkAndCalibrate
    }

    public enum CalibrationStatus
    {
        NotColibated,
        Calibrating,
        Colibated,
        Error,
        Break
    }

    public enum Sensors
    {
        ZeroMark,
        Overcurrent
    }

    public enum SensorsBits
    {
        ZeroMark = 1 << Sensors.ZeroMark,
        Overcurrent = 1 << Sensors.Overcurrent,
    }

    public unsafe struct StatusT : IResponseAdapter
    {
        public uint Value;

        public SensorsBits Sensors => (SensorsBits)((Value) & 0x03);
        public MotionState MotionState => (MotionState)((Value >> 2) & 0x0f);
        public MotionError MotionError => (MotionError)((Value >> 6) & 0x0f);
        public CalibrationStatus Calibration => (CalibrationStatus)((Value >> 10) & 0x0f);
        public CalibrationState CalibrationState => (CalibrationState)((Value >> 14) & 0x0f);

        public unsafe object Recieve(xContent content)
        {
            this = *(StatusT*)content.Data;

            return this;
        }
    }

    public enum SpeedCalibrationInfo : int
    {
        PolynomialSize = 4
    }

    public struct CalibrationT : IResponseAdapter, IRequestAdapter
    {
        public float Position;
        public float Offset;

        public unsafe object Recieve(xContent content)
        {
            this = *(CalibrationT*)content.Data;

            return this;
        }

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(CalibrationT);
        }
    }

    public enum SetPositionMode
    {
        Commom,
        StopAtZeroMark,
        OutAtZeroMark,
        FindZeroMark,
    }

    public struct RequestSetPositionT : IRequestAdapter
    {
        public float Angle;
        public float Power;

        public int Time;
        public uint Mode;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestSetPositionT);
        }
    }

    public struct RequestSetPodT : IRequestAdapter
    {
        public byte Number;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestSetPodT);
        }
    }
}
