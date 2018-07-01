using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Captura.Native;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;

namespace Captura
{
    public static class ImageFromClipboard
    {
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
                            Gdi32.DeleteObject(hBitmap);
                        }
                    }
                }
            }

            return null;
        }
    }
}
