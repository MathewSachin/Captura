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
        public static Rectangle CreateRectangle(int Left, int Top, int Right, int Bottom) 
        {
            return new Rectangle(Left, Top, Right - Left, Bottom - Top);
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
