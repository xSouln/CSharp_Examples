using Camera.Communication;
using Camera.Communication.Transactions;
using Camera.UI.Adapters;
using Camera.UI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using xLib.Common;
using xLib.Transceiver;
using xLib.UI;

namespace Camera.UI
{
    public class Snapshot : ModelBase<Control, SnapshotViewModel>
    {
        protected List<uint> RGB565 = new List<uint>();
        protected List<byte> JPEG = new List<byte>();

        public Snapshot(Control control) : base(control)
        {
            Name = "Snapshot";

            Events.SnapshotJPEG_TransferStart.EventReceive += SnapshotJPEG_TransferStart;
            Events.SnapshotJPEG_TransferSegment.EventReceive += SnapshotJPEG_TransferSegment;
            Events.SnapshotJPEG_TransferEnd.EventReceive += SnapshotJPEG_TransferEnd;

            Events.SnapshotRGB565_TransferStart.EventReceive += SnapshotRGB565_TransferStart;
            Events.SnapshotRGB565_TransferSegment.EventReceive += SnapshotRGB565_TransferSegment;
            Events.SnapshotRGB565_TransferEnd.EventReceive += SnapshotRGB565_TransferEnd;

            ViewModel = new SnapshotViewModel(this);
        }

        public override void Dispose()
        {
            ViewModel.Dispose();
        }

        internal unsafe void SnapshotRGB565_TransferSegment(object obj, Events.EventSnapshot arg)
        {
            for (int i = 0; i < arg.PixelsCount; i++)
            {
                RGB565.Add(arg.Pixels[i]);
            }
        }

        internal void SnapshotRGB565_TransferStart(object obj, Events.EventResult<ActionResult> arg)
        {
            RGB565.Clear();
        }

        internal void SnapshotRGB565_TransferEnd(object obj, Events.EventResult<ActionResult> arg)
        {
            ViewModel.Load_RGB565(RGB565.ToArray());
        }

        protected void SnapshotJPEG_TransferEnd(object obj, Events.EventResult<ActionResult> arg)
        {
            int i = JPEG.Count;
            while (i > 0)
            {
                i--;
                if (JPEG[i] != 0xd9)
                {
                    JPEG.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            ViewModel.Load_JPEG(JPEG.ToArray());

            xTracer.Message(nameof(SnapshotJPEG_TransferEnd));
        }

        protected unsafe void SnapshotJPEG_TransferSegment(object obj, Events.EventJPEG arg)
        {
            for (int i = 0; i < arg.DataSize; i++)
            {
                JPEG.Add(arg.Data[i]);
            }
        }

        protected void SnapshotJPEG_TransferStart(object obj, Events.EventResult<ActionResult> arg)
        {
            JPEG.Clear();
        }
    }
}
