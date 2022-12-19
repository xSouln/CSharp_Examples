using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Transceiver
{
    public struct PacketIdentificatorT
    {
        /// <summary>
        /// byte StartKey = '#'
        /// ushort Description = description of the purpose of the package - request, response, event, etc.
        /// byte EndKey = ':'
        /// </summary>
        public uint Value; // format: [#][Description][:]
    }

    public struct PacketHeaderT
    {
        /// <summary>
        /// byte StartKey = '#'
        /// ushort Description = description of the purpose of the package - request, response, event, etc.
        /// byte EndKey = ':'
        /// </summary>
        public uint Identificator; // format: [#][Description][:]

        /// <summary>
        /// unique key of the device, module. 0 - system commands
        /// </summary>
        public uint DeviceKey;

        public static PacketHeaderT Init(uint identificator, PacketHeaderDescription value, uint device_key)
        {
            return new PacketHeaderT
            {
                Identificator = (identificator & (uint)PacketIdentificator.Mask) | ((uint)value << 8),
                DeviceKey = device_key
            };
        }

        public static PacketHeaderT Init(PacketHeaderDescription value, uint device_key)
        {
            return new PacketHeaderT
            {
                Identificator = (uint)PacketIdentificator.Default | ((uint)value << 8),
                DeviceKey = device_key
            };
        }

        public static PacketHeaderT Init(PacketHeaderDescription value)
        {
            return new PacketHeaderT
            {
                Identificator = (uint)PacketIdentificator.Default | ((uint)value << 8),
            };
        }
    }

    public struct PacketInfoT
    {
        /// <summary>
        /// generated key - when receiving a response, must match the request key
        /// </summary>
        public uint RequestId;

        /// <summary>
        /// action(command) key
        /// </summary>
        public ushort Action;

        /// <summary>
        /// size of nested data after packet info
        /// </summary>
        public ushort ContentSize;
    }

    /// <summary>
    /// array: [#][Description][:][DeviceKey][RequestId][ActionKey][ContentSize][uint8_t Content[ContentSize]][\r]
    /// </summary>
    public struct PacketT
    {
        public PacketHeaderT Header; //format: [#][Description][:][DeviceKey]
        public PacketInfoT Info; //format: [RequestId][Action][ContentSize]
        //byte Content[Info.ContentSize]
        //byte EndPacketSymbol default('\r')
    }

    public enum PacketIdentificator : uint
    {
        Mask = 0xFF0000FF,
        Default = 0x2300003A,

        DescriptionMask = 0x00FFFF00,
    }

    public enum PacketHeaderDescription : ushort
    {
        Request = 0x5251, //"RQ"
        Response = 0x5253, //"RS"
        Event = 0x4554, //"ET"
        Error = 0x4552 //"ER"
    }

    public class PacketBase
    {
        public static unsafe void Add<TData>(List<byte> packet, TData data) where TData : unmanaged
        {
            if (packet != null)
            {
                byte* ptr = (byte*)&data;

                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Add(ptr[i]);
                }
            }
        }

        public static unsafe void Add<TData>(List<byte> packet, TData[] data) where TData : unmanaged
        {
            if (packet != null)
            {
                foreach (TData element in data)
                {
                    byte* ptr = (byte*)&element;

                    for (int i = 0; i < sizeof(TData); i++)
                    {
                        packet.Add(ptr[i]);
                    }
                }
            }
        }

        public static unsafe void Add(List<byte> packet, void* ptr, int size)
        {
            if (packet != null)
            {
                byte* _ptr = (byte*)ptr;

                while (size > 0)
                {
                    packet.Add(*_ptr);
                    size--;
                }
            }
        }

        public static void Add(List<byte> packet, string data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }

        public static void Add(List<byte> packet, byte[] data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }
    }
}
