using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.Types
{
    public class DeviceInfo
    {
        public const uint Address = 0x63171134;
    }

    public enum Saturations : byte
    {
        Level_0,

        Level_P1,
        Level_P2,

        Level_M1,
        Level_M2,
    }

    public enum Contrasts : byte
    {
        Level_0,

        Level_P1,
        Level_P2,

        Level_M1,
        Level_M2,
    }

    public enum Brightnesses : byte
    {
        Level_0,

        Level_P1,
        Level_P2,

        Level_M1,
        Level_M2,
    }

    public enum LightModes : byte
    {
        Auto,
        Sunny,
        Cloudy,
        Office,
        Home,
    }

    public enum SpecialEffects : byte
    {
        Normal,

        Black,
        Negative,
        BlackNegative,

        Bluish,
        Greenish,
        Reddish,

        Antique,
    }

    public enum Quantizations : byte
    {
        Default = 0x0c
    }

    public enum Resolutions : byte
    {
        _160x120,
        _320x240,
        _640x480,
        _800x600,
        _1024x768,
        _1280x960,
    }

    public enum OutputFormats : byte
    {
        JPEG,
        YUV422,
        RGB565,
        RAW,
    }

    public struct OptionsT
    {
        public OutputFormats OutputFormat;
        public Resolutions Resolution;

        public Contrasts Contrast;
        public Saturations Saturation;
        public Brightnesses Brightness;

        public LightModes LightMode;
        public SpecialEffects SpecialEffect;
        public byte Quantization;

        public byte AGC_Gain;
    }

    [Serializable]
    public struct ConfigurationElement
    {
        public byte Register { get; set; }
        public byte Value { get; set; }
    }
}
