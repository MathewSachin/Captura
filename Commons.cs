using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using ManagedWin32.Api;
using SharpAvi;

namespace Captura
{
    public static class Commons
    {
        public static readonly FourCC GifFourCC = new FourCC("_gif");

        public static Rectangle CreateRectangle(int Left, int Top, int Right, int Bottom) 
        {
            return new Rectangle(Left, Top, Right - Left, Bottom - Top);
        }

        public static readonly int DesktopHeight, DesktopWidth;

        public static readonly Rectangle DesktopRectangle;

        public static readonly IntPtr DesktopHandle = User32.GetDesktopWindow();

        static Commons()
        {
            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                toDevice = source.CompositionTarget.TransformToDevice;

            DesktopHeight = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight * toDevice.M22);
            DesktopWidth = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenWidth * toDevice.M11);

            DesktopRectangle = new Rectangle(0, 0, DesktopWidth, DesktopHeight);
        }

        public static Color ConvertColor(System.Windows.Media.Color C) { return System.Drawing.Color.FromArgb(C.A, C.R, C.G, C.B); }

        public static void WriteToClipboard(this Bitmap BMP, bool PreserveTransparency)
        {
            if (PreserveTransparency)
            {
                using (var PngStream = new MemoryStream())
                {
                    BMP.Save(PngStream, ImageFormat.Png);
                    var pngClipboardData = new DataObject("PNG", PngStream);

                    using (var whiteS = new Bitmap(BMP.Width, BMP.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        using (var graphics = Graphics.FromImage(whiteS))
                        {
                            graphics.Clear(Color.White);
                            graphics.DrawImage(BMP, 0, 0, BMP.Width, BMP.Height);
                        }

                        // Add fallback for applications that don't support PNG from clipboard (eg. Photoshop or Paint)
                        pngClipboardData.SetData(DataFormats.Bitmap, whiteS);

                        Clipboard.Clear();
                        Clipboard.SetDataObject(pngClipboardData, true);
                    }
                }
            }
            else Clipboard.SetImage(BMP);
        }
    }
}
