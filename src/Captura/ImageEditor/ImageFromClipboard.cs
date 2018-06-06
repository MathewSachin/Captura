using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;

namespace Captura
{
    public static class ImageFromClipboard
    {
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr Object);

        public static BitmapSource Get()
        {
            if (Clipboard.ContainsImage())
            {
                var clipboardData = Clipboard.GetDataObject();

                if (clipboardData != null)
                {
                    if (clipboardData.GetDataPresent(DataFormats.Bitmap))
                    {
                        var bitmap = (Bitmap)clipboardData.GetData(DataFormats.Bitmap);
                        var hBitmap = bitmap.GetHbitmap();

                        try
                        {
                            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        }
                        finally
                        {
                            DeleteObject(hBitmap);
                        }
                    }
                }
            }

            return null;
        }
    }
}
