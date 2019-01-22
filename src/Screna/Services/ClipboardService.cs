using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ClipboardService : IClipboardService
    {
        public void SetText(string Text)
        {
            if (Text == null)
                return;

            try { Clipboard.SetText(Text); }
            catch (ExternalException)
            {
                ServiceProvider.MessageProvider?.ShowError($"Copy to Clipboard failed:\n\n{Text}");
            }
        }

        public string GetText() => Clipboard.GetText();

        public bool HasText => Clipboard.ContainsText();

        public void SetImage(Image Bmp)
        {
            using (var pngStream = new MemoryStream())
            {
                Bmp.Save(pngStream, ImageFormat.Png);
                var pngClipboardData = new DataObject("PNG", pngStream);

                using (var whiteS = new Bitmap(Bmp.Width, Bmp.Height, PixelFormat.Format24bppRgb))
                {
                    using (var graphics = Graphics.FromImage(whiteS))
                    {
                        graphics.Clear(Color.White);
                        graphics.DrawImage(Bmp, 0, 0, Bmp.Width, Bmp.Height);
                    }

                    // Add fallback for applications that don't support PNG from clipboard (eg. Photoshop or Paint)
                    pngClipboardData.SetData(DataFormats.Bitmap, whiteS);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(pngClipboardData, true);
                }
            }
        }

        public Image GetImage() => Clipboard.GetImage();

        public bool HasImage => Clipboard.ContainsImage();
    }
}