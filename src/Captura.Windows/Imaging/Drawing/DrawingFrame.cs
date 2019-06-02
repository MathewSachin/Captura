using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Captura;

namespace Screna
{
    public class DrawingFrame : IBitmapFrame
    {
        public Bitmap Bitmap { get; }

        public TimeSpan Timestamp { get; }

        public DrawingFrame(Bitmap Bitmap, TimeSpan Timestamp)
        {
            this.Timestamp = Timestamp;
            this.Bitmap = Bitmap;
            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }

        DrawingFrame() { }

        public static IBitmapFrame DummyFrame { get; } = new DrawingFrame();

        public void Dispose() => Bitmap.Dispose();

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer)
        {
            var bits = Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                Marshal.Copy(bits.Scan0, Buffer, 0, Width * Height * 4);
            }
            finally
            {
                Bitmap.UnlockBits(bits);
            }
        }

        public void CopyTo(IntPtr Buffer)
        {
            var bits = Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                Kernel32.CopyMemory(Buffer, bits.Scan0, Width * Height * 4);
            }
            finally
            {
                Bitmap.UnlockBits(bits);
            }
        }
    }
}