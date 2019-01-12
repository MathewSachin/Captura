using System;
using System.Drawing;
using System.Drawing.Imaging;
using Captura;

namespace Screna
{
    public class GraphicsBitmapLoader : IBitmapLoader
    {
        GraphicsBitmapLoader() { }

        public static GraphicsBitmapLoader Instance { get; } = new GraphicsBitmapLoader();

        public IDisposable CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            return new Bitmap(Size.Width, Size.Height, Stride, PixelFormat.Format32bppRgb, MemoryData);
        }

        public IDisposable LoadBitmap(string FileName, out Size Size)
        {
            var bmp = new Bitmap(FileName);

            Size = bmp.Size;

            return bmp;
        }

        public void Dispose() { }
    }
}