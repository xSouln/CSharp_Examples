using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Transceiver;

namespace CupsControl.Types
{
    public class DeviceInfo
    {
        public const uint Address = 0x55677380;
    }

    public interface IDataAdapter
    {
        int GetDataSize();
        byte[] GetData();
    }

    public struct ColorT
    {
        public byte Green { get; set; }
        public byte Red { get; set; }
        public byte Blue { get; set; }
    }

    public enum Cups : byte
    {
        Cup1,
        Cup2,
        Cup3,
        Cup4,
        Count
    }

    public enum CupNumberBits : byte
    {
        Cup1 = 1 << Cups.Cup1,
        Cup2 = 1 << Cups.Cup2,
        Cup3 = 1 << Cups.Cup3,
        Cup4 = 1 << Cups.Cup4,
    }

    public enum TemplateId : byte
    {
        WhiteBlink,
        RedBlink
    }

    public enum StatusBits : uint
    {
        DrawingIsEnable = 1 << 0,
    }

    public struct StatusT : IResponseAdapter
    {
        public StatusBits Value;

        public unsafe object Recieve(xContent content)
        {
            this = *(StatusT*)content.Data;
            return this;
        }

        public bool IsEnable(StatusBits bits)
        {
            return (Value & bits) == bits;
        }
    }

    public unsafe class ResponseGetStatus : IResponseAdapter
    {
        public StatusT* Values;
        public int ValuesCount;

        public unsafe object Recieve(xContent content)
        {
            Values = (StatusT*)content.Data;
            ValuesCount = content.DataSize / sizeof(StatusT);
            return this;
        }
    }

    public struct RequestSetPixelsT
    {
        public CupNumberBits Cups;
        public ushort StartPixelIndex;
        public ushort PixelsCount;
        //public PixelT[PixelsCount] Pixels;
    }

    public struct RequestSetTemplateT : IDataAdapter
    {
        public CupNumberBits Cups;
        public Template Template;

        public byte[] GetData()
        {
            return Memory.ToByteArray(this);
        }

        public unsafe int GetDataSize()
        {
            return sizeof(RequestSetTemplateT);
        }
    }

    public struct RequestGetPixelsT
    {
        public Cups CupNumber;
        /*
        public unsafe byte[] GetData()
        {
            byte[] data = new byte[sizeof(RequestGetPixelsT)];
            return Memory.ToByteArray(this);
        }

        public unsafe int GetDataSize()
        {
            return sizeof(RequestGetPixelsT);
        }
        */
    }

    public enum Template : byte
    {
        Template_1,
        Template_2,
    }

    public struct RequestSetColorT
    {
        public CupNumberBits Selector { get; set; }
        public ColorT Color { get; set; }
    }

    public struct RequestSetTemplateByIdT
    {
        public CupNumberBits Selector { get; set; }
        public TemplateId TemplateId { get; set; }
    }

    public class RequestSetPixels : IDataAdapter
    {
        public CupNumberBits Cups;
        public ushort StartPixelIndex;
        ColorT[] Pixels;

        public RequestSetPixels(CupNumberBits cups, ushort start_pixel_index, ColorT[] pixels)
        {
            Cups = cups;
            StartPixelIndex = start_pixel_index;
            Pixels = pixels;
        }

        public unsafe byte[] GetData()
        {
            List<byte> data = new List<byte>();
            //Memory.
            return data.ToArray();
        }

        public unsafe int GetDataSize()
        {
            return sizeof(CupNumberBits) + sizeof(ushort) + (sizeof(ColorT) * Pixels.Length);
        }
    }
}
