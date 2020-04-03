using System.Drawing;
using System.IO;

namespace Captura.Windows.Gdi
{
    class DrawingImagingSystem : IImagingSystem
    {
        public IBitmapImage CreateBitmap(int Width, int Height)
        {
            return new DrawingImage(new Bitmap(Width, Height));
        }

        public IBitmapImage LoadBitmap(string FileName)
        {
            return new DrawingImage(new Bitmap(FileName));
        }

        public IBitmapImage LoadBitmap(Stream Stream)
        {
            return new DrawingImage(new Bitmap(Stream));
        }
    }
}