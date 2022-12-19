using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLib.Transceiver;
using xLib.UI;

namespace xLib.Common
{
    public static class xTracer
    {
        public static ObservableCollection<ReceivePacketInfo> Notes { get; set; } = new ObservableCollection<ReceivePacketInfo>();
        public static ObservableCollection<ReceivePacketInfo> Info { get; set; } = new ObservableCollection<ReceivePacketInfo>();
        public static ActionAccessUI PointEntryUI = xSupport.ActionThreadUI;
        public static bool Pause;

        public static void Message(string note, string data, string convert_data)
        {
            if (Pause) { return; }
            try
            {
                PointEntryUI?.Invoke((RequestUI) =>
                {
                    if (Info.Count > 500) Info.RemoveAt(0);
                    /*
                    Info.Insert(0, new ReceivePacketInfo
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Note = note,
                        Data = data
                    });
                    */
                    Info.Add(new ReceivePacketInfo
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Note = note,
                        Data = data
                    });
                }, null);
            }
            catch { }
        }

        public static void Message(TransmitterBase request, string name)
        {
            if (request != null)
            {
                Message(name + " => response: \"" + request.Name + "\"; result: " + request.Status + "; response time: " + request.ResponseTime);
            }
            else
            {
                Message(" request = null");
            }
        }

        public static void MessageInfo(string info)
        {
            if (Pause) { return; }
            try
            {
                PointEntryUI?.Invoke((RequestUI) =>
                {
                    if (Info.Count > 500) Info.RemoveAt(0);

                    Info.Add(new ReceivePacketInfo
                    {
                        Time = DateTime.Now.ToUniversalTime().ToString(),
                        Note = info,
                    });
                }, null);
            }
            catch { }
        }

        public static void Message(string data) { Message("info:", data, null); }
        public static void Message(string data, string convert_data) { Message("info:", data, convert_data); }

    }
}
