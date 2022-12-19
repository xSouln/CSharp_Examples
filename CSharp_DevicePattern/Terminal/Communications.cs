using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib;
using xLib.Common;
using xLib.Controls;
using xLib.Net;
using xLib.Ports;
using xLib.Transceiver;
using xLib.Types;

namespace Terminal
{
    public partial class Control
    {
        public TCPClient TCP = new TCPClient()
        {
            Receiver = new xObjectReceiver(0x3fff, new byte[] { (byte)'\r' })
        };

        public xSerialPort SerialPort = new xSerialPort()
        {
            Receiver = new xObjectReceiver(0x1fff, new byte[] { (byte)'\r' })
        };

        public override xAction<bool, byte[]> GetTransmitter()
        {
            if (SerialPort.Port != null && SerialPort.Port.IsOpen)
            {
                return SerialPort.Send;
            }
            else if (TCP.ConnectionState == ConnectionState.Connected)
            {
                return TCP.Send;
            }
            return null;
        }

        public unsafe xObjectReceiver.Result PacketReceiver(xObjectReceiver rx, byte* data, int data_size)
        {
            //string convert = "";
            string temp = "";

            //casting data to package structure
            PacketT* packet = (PacketT*)data;

            xContent content = new xContent
            {
                Data = data,
                DataSize = data_size
            };

            //whether the package is a transaction
            if (data_size >= sizeof(PacketIdentificator) && (packet->Header.Identificator & (uint)PacketIdentificator.Mask) == (uint)PacketIdentificator.Default)
            {
                //size check for minimum transaction length
                if (data_size < sizeof(PacketT))
                {
                    return xObjectReceiver.Result.Storage;
                }

                int content_size = data_size - sizeof(PacketT);

                //checking if the package content size matches the actual size, if the size is short
                if (content_size < packet->Info.ContentSize)
                {
                    return xObjectReceiver.Result.Storage;
                }

                //reset size when content exceeds size specified in packet.Info
                if (content_size > packet->Info.ContentSize)
                {
                    xTracer.Message("transaction content size error");
                    goto end;
                }

                foreach (DeviceBase device in Devices)
                {
                    if (device.ResponseIdentification(content))
                    {
                        goto end;
                    }
                }
            }
            /*
            if (SomeDevice.Requests.Identification(content))
            {
                goto end;
            }

            if (SomeDevice.Responses.Identification(content))
            {
                goto end;
            }

            if (Camera.ResponseIdentification(content))
            {
                goto end;
            }

            if (Carousel.ResponseIdentification(content))
            {
                goto end;
            }
            */
            //convert = xConverter.ToStrHex(data, data_size);
            temp = xConverter.GetString(data, data_size);

            xTracer.Message("unidentified data: " + temp);

        end:
            return xObjectReceiver.Result.Reset;
        }
    }
}
