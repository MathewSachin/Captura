using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Clipboard = System.Windows.Forms.Clipboard;

namespace Captura
{
    public static class ImageFromClipboard
    {
        public static BitmapSource Get()
        {
            if (Clipboard.ContainsImage() && Clipboard.GetImage() is Bitmap bitmap)
            {
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);

                return BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
            }

            return null;
        }
    }
}
