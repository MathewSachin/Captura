using System.Drawing;
using System.IO;

namespace Captura.Windows.Gdi
{
    public class DrawingImage : IBitmapImage
    {
        public Image Image { get; }

        public DrawingImage(Image Image)
        {
            this.Image = Image;
        }

        public void Dispose()
        {
            Image.Dispose();
        }

        public int Width => Image.Width;
        public int Height => Image.Height;

        public void Save(string FileName, ImageFormats Format)
        {
            Image.Save(FileName, Format.ToDrawingImageFormat());
        }

        public void Save(Stream Stream, ImageFormats Format)
        {
            Image.Save(Stream, Format.ToDrawingImageFormat());
        }
    }
}