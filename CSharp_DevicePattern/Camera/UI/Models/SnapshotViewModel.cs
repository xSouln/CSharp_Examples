using Camera.Communication.Transactions;
using Camera.UI.Adapters;
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

namespace Camera.UI.Models
{
    public class SnapshotViewModel : ViewModelBase<Control, Snapshot>
    {
        protected GridAdapter buttons_grid { get; set; } = new GridAdapter() { Background = UIProperty.GetBrush("#FF3F3F46") };
        protected ListViewAdapter list_view;
        protected BitmapImage image;

        protected uint snapshot_number = 0;
        protected uint snapshot_session_key = 0;

        public SnapshotViewModel(Snapshot parent) : base(parent.Control, parent)
        {
            buttons_grid.Add(new ButtonAdapter("JPEG Snapshot", SnapshotJPEG_ClickEvent));
            buttons_grid.Add(null);
            buttons_grid.Add(new ButtonAdapter("Reset number", ResetNumber_ClickEvent));
            buttons_grid.Add(new ButtonAdapter("Generate key", GenerateSessionKey_ClickEvent));

            var grid = new Grid();
            var image = new ImageAdapter(nameof(Image));

            grid.Margin = new Thickness { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            Grid.SetColumn(image, 0);
            Grid.SetRow(image, 0);

            Grid.SetColumn(buttons_grid, 1);
            Grid.SetRow(buttons_grid, 0);

            grid.Children.Add(image);
            grid.Children.Add(buttons_grid);

            UIModel = grid;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public BitmapImage Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private async void SnapshotJPEG_ClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Get.SnaphotJPEG.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300);
            TransmitterBase.Trace(nameof(SnapshotJPEG_ClickEvent), request);
        }

        private void ResetNumber_ClickEvent(object sender, RoutedEventArgs e)
        {
            snapshot_number = 0;
        }

        private void GenerateSessionKey_ClickEvent(object sender, RoutedEventArgs e)
        {
            snapshot_session_key = (uint)new Random().Next();
        }

        private async void SnapshotRGB565_ClickEvent(object sender, RoutedEventArgs e)
        {
            var request = await Get.SnaphotRGB565.Prepare(Control.Requests.Handle).TransmitionAsync(Control.Terminal.GetTransmitter(), 1, 300);
            TransmitterBase.Trace(nameof(SnapshotRGB565_ClickEvent), request);
        }

        public int Load_JPEG(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                string name = "Snapshot_" + snapshot_session_key + "_" + snapshot_number + ".jpeg";//"Snapshot" + options + time + ".jpeg";

                try
                {
                    File.Delete(name);
                }
                catch
                {

                }

                string time = "(Date=" + DateTime.Now.ToUniversalTime().ToString().Replace(":", ".") + ")";
                time = time.Replace(" ", ",Time=");
                string options = "(R=" + ("" + Control.Driver.Options.Resolution.Value).Trim('_') + ",AGC=" + Control.Driver.Options.AGC_Gain.Value + ",Q=" + Control.Driver.Options.Quantization.Value + ")";
                snapshot_number++;

                try
                {
                    using (FileStream Stream = new FileStream(name, FileMode.OpenOrCreate))
                    {
                        Stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    xTracer.Message("jpeg FileStream: " + ex);
                    return -1;
                }

                MemoryStream memory = new MemoryStream(data);
                try
                {
                    System.Drawing.Image image = System.Drawing.Image.FromStream(memory);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Image = bitmapImage;
                }
                catch (Exception ex)
                {
                    xTracer.Message("jpeg snapshot: " + ex);
                    return -1;
                }

                return 0;
            }
            return -1;
        }

        public int Load_RGB565(uint[] data)
        {
            if (data != null && data.Length > 0)
            {
                int width = 240;
                int height = 320;

                Color[] pixels = rgb565_to_color(data);

                if (pixels == null)
                {
                    return -1;
                }

                var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                /*
                var bmpData = bmp.LockBits(
                                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     ImageLockMode.WriteOnly, bmp.PixelFormat);
                */
                // Get an ImageCodecInfo object that represents the JPEG codec.
                ImageCodecInfo image_codec_info = GetEncoderInfo("image/jpeg");

                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                // Create an EncoderParameters object.

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bmp.SetPixel(x, y, pixels[(y + 1) * x]);
                    }
                }

                // EncoderParameter object in the array.
                EncoderParameters encoder_parameters = new EncoderParameters(1);
                EncoderParameter encoder_parameter = new EncoderParameter(encoder, 80L);
                encoder_parameters.Param[0] = encoder_parameter;

                bmp.Save("Shapes075.jpeg", image_codec_info, encoder_parameters);

                using (var memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Image = bitmapImage;
                }
                return 0;
            }
            return -1;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public Color[] rgb565_to_color(uint[] data)
        {
            Color[] pixels = null;

            if (data != null)
            {
                pixels = new Color[data.Length * 2];

                int i = 0;
                int red;
                int green;
                int blue;
                foreach (uint element in data)
                {
                    ushort pixel;// = (ushort)(((element >> 8) & 0xff) | ((element << 8) & 0xff00));
                    pixel = (ushort)element;

                    red = (pixel >> 11) & 0x1f;
                    green = (pixel >> 5) & 0x3f;
                    blue = pixel & 0x1f;
                    /*
                    pixels[i] = Color.FromArgb(0xff * red / 0x1f,
                                                0xff * green / 0x3f,
                                                0xff * blue / 0x1f);
                    */

                    //pixels[i] = Color.FromArgb(red << 3, green << 2, blue << 3);
                    pixels[i] = Color.FromArgb((red * 527 + 23) >> 6,
                        (green * 259 + 33) >> 6,
                        (blue * 527 + 23) >> 6);
                    i++;

                    pixel = (ushort)(element >> 16);
                    //pixel = (ushort)(((element >> 24) & 0xff) | ((element >> 8) & 0xff00));

                    red = (pixel >> 11) & 0x1f;
                    green = (pixel >> 5) & 0x3f;
                    blue = pixel & 0x1f;

                    //pixels[i] = Color.FromArgb(red << 3, green << 2, blue << 3);
                    pixels[i] = Color.FromArgb((red * 527 + 23) >> 6,
                        (green * 259 + 33) >> 6,
                        (blue * 527 + 23) >> 6);
                    i++;
                }
            }
            return pixels;
        }
    }
}
