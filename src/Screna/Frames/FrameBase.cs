using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Captura;

namespace Screna
{
    public abstract class FrameBase : IBitmapFrame
    {
        public Bitmap Bitmap { get; }

        protected FrameBase(Bitmap Bitmap)
        {
            this.Bitmap = Bitmap;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }

        public abstract void Dispose();

        public void SaveGif(Stream Stream)
        {
            Bitmap.Save(Stream, ImageFormat.Gif);
        }

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer, int Length)
        {
            var bits = Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            Parallel.For(0, Height, Y =>
            {
                var absStride = Math.Abs(bits.Stride);

                Marshal.Copy(bits.Scan0 + (Y * bits.Stride), Buffer, Y * absStride, absStride);
            });

            Bitmap.UnlockBits(bits);
        }

        public abstract IBitmapEditor GetEditor();
    }
}