using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Captura.Models;
using Captura.Windows.Gdi;

namespace Captura.Windows
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class ClipboardService : IClipboardService
    {
        readonly IMessageProvider _messageProvider;

        public ClipboardService(IMessageProvider MessageProvider)
        {
            _messageProvider = MessageProvider;
        }

        public void SetText(string Text)
        {
            if (Text == null)
                return;

            try { Clipboard.SetText(Text); }
            catch (ExternalException)
            {
                _messageProvider?.ShowError($"Copy to Clipboard failed:\n\n{Text}");
            }
        }

        public string GetText() => Clipboard.GetText();

        public bool HasText => Clipboard.ContainsText();

        public void SetImage(IBitmapImage Bmp)
        {
            using var pngStream = new MemoryStream();
            Bmp.Save(pngStream, ImageFormats.Png);
            var pngClipboardData = new DataObject("PNG", pngStream);

            using var whiteS = new Bitmap(Bmp.Width, Bmp.Height, PixelFormat.Format24bppRgb);
            Image drawingImg;

            if (Bmp is DrawingImage drawingImage)
                drawingImg = drawingImage.Image;
            else drawingImg = Image.FromStream(pngStream);

            using (var graphics = Graphics.FromImage(whiteS))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(drawingImg, 0, 0, Bmp.Width, Bmp.Height);
            }

            // Add fallback for applications that don't support PNG from clipboard (eg. Photoshop or Paint)
            pngClipboardData.SetData(DataFormats.Bitmap, whiteS);

            Clipboard.Clear();
            Clipboard.SetDataObject(pngClipboardData, true);
        }

        public IBitmapImage GetImage() => new DrawingImage(Clipboard.GetImage());

        public bool HasImage => Clipboard.ContainsImage();
    }
}