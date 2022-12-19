using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace Slider.Types
{
    public class DeviceInfo
    {
        public const uint Address = 0x55882435;
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
        public float StartSpeed;
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

    public enum MoveMode : byte
    {
        Common,
        MoveOffTheSensor
    }

    public enum MovePosition : byte
    {
        Close,
        Open
    }

    public enum PodRealiseResult
    {
        NoError,
        Timeout,
        Overcurrent,
        Break
    }

    public enum PodRealiseState
    {
        Idle,
        Opening,
        IsOpen,
        Closing,
        MoveOffTheSensor,
        Realised
    }

    public struct PodRealiseStatusT
    {
        public uint Value;

        public PodRealiseState State => (PodRealiseState)(Value & 0x0f);
        public PodRealiseResult Result => (PodRealiseResult)((Value >> 4) & 0x0f);
        public Result InitResult => (Result)((Value >> 8) & 0x0f);
    }

    public enum Sensors : byte
    {
        Closing,
        Open,
        Overcurrent
    }

    public enum SensorsState : byte
    {
        Closing = 1 << Sensors.Closing,
        Open = 1 << Sensors.Open,
        Overcurrent = 1 << Sensors.Overcurrent
    }

    public struct SensorsStateT
    {
        public byte Value;

        public bool Close => (Value & (byte)SensorsState.Closing) == (byte)SensorsState.Closing;
        public bool Open => (Value & (byte)SensorsState.Open) == (byte)SensorsState.Open;
        public bool Overcurrent => (Value & (byte)SensorsState.Overcurrent) == (byte)SensorsState.Overcurrent;
    }

    public enum MotionState
    {
        Stopped,
        Opening,
        Closing
    }

    public enum MotionResult
    {
        NoError,
        Timeout,
        Overcurrent,
    }

    public struct MotorStatusT
    {
        public byte Value;

        public MotionState State => (MotionState)(Value & 0x0f);
        public MotionResult Result => (MotionResult)((Value >> 4) & 0x0f);
    }

    public struct StatusT : IResponseAdapter
    {
        public PodRealiseStatusT PodRealise;
        public SensorsStateT Sensors;
        public MotorStatusT Motor;

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
