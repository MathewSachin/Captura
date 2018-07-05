using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Captura.Native;
using Clipboard = System.Windows.Forms.Clipboard;

namespace Captura
{
    public static class ImageFromClipboard
    {
        public static BitmapSource Get()
        {
            if (Clipboard.ContainsImage() && Clipboard.GetImage() is Bitmap bitmap)
            {
                var hBitmap = bitmap.GetHbitmap();

                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    Gdi32.DeleteObject(hBitmap);
                }
            }

            return null;
        }
    }
}
