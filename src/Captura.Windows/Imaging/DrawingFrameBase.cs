using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Captura;

namespace Screna
{
    public abstract class DrawingFrameBase : IBitmapFrame
    {
        public Bitmap Bitmap { get; }

        protected DrawingFrameBase(Bitmap Bitmap)
        {
            this.Bitmap = Bitmap;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }

        public abstract void Dispose();

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer, int Length)
        {
            var bits = Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                Marshal.Copy(bits.Scan0, Buffer, 0, Length);
            }
            finally
            {
                Bitmap.UnlockBits(bits);
            }
        }
    }
}