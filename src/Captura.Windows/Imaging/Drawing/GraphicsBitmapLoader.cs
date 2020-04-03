using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captura.Windows.Gdi
{
    public class GraphicsBitmapLoader : IBitmapLoader
    {
        GraphicsBitmapLoader() { }

        public static GraphicsBitmapLoader Instance { get; } = new GraphicsBitmapLoader();

        public IBitmapImage CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            var bmp = new Bitmap(Size.Width, Size.Height, Stride, PixelFormat.Format32bppRgb, MemoryData);

            return new DrawingImage(bmp);
        }

        public IBitmapImage LoadBitmap(string FileName)
        {
            var bmp = new Bitmap(FileName);

            return new DrawingImage(bmp);
        }

        public void Dispose() { }
    }
}