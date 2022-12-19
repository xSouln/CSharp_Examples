using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace FlowDirector.Types
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
        tLinkError,
        ComponentInitializationError,
    }

    public struct OptionsT : IRequestAdapter, IResponseAdapter
    {
        public float Acceleration;
        public float Deceleration;

        public float StartSpeed;
        public float StopSpeed;

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
        StartingMove,
        InMove,
        Complite,
        Error
    }

    public enum MotionError
    {
        NoError,
        Busy,
    }

    public enum Sensors
    {
        Sensor1 = 1 << 0,
        Sensor2 = 1 << 1,
    }

    public struct StatusT : IResponseAdapter
    {
        public ulong Value;

        public MotionState MotionState => (MotionState)((Value >> 4) & 0x0f);
        public Sensors Sensors => (Sensors)((Value) & 0x0f);
        public MotionError MotionError => (MotionError)((Value >> 8) & 0x07);


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

    public unsafe struct SpeedCalibrationT
    {
        public fixed float Polynomial[(int)SpeedCalibrationInfo.PolynomialSize];
    }

    public struct CalibrationT : IResponseAdapter, IRequestAdapter
    {
        public float Position;
        public SpeedCalibrationT Speed;

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
}
