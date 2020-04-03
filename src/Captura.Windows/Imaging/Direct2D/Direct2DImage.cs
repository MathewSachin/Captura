using System.IO;
using SharpDX.Direct2D1;

namespace Captura.Windows.DirectX
{
    public class Direct2DImage : IBitmapImage
    {
        public Bitmap Bitmap { get; }

        public Direct2DImage(Bitmap Bitmap)
        {
            this.Bitmap = Bitmap;

            var size = Bitmap.PixelSize;

            Width = size.Width;
            Height = size.Height;
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public int Width { get; }
        public int Height { get; }

        public void Save(string FileName, ImageFormats Format)
        {
            throw new System.NotImplementedException();
        }

        public void Save(Stream Stream, ImageFormats Format)
        {
            throw new System.NotImplementedException();
        }
    }
}