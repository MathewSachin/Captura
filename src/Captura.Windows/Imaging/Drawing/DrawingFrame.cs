using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Captura.Native;

namespace Captura.Windows.Gdi
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

        void Copy(Action<(IntPtr SrcPtr, int DestOffset, int Length)> Copier)
        {
            var bits = Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var rowSize = Math.Abs(bits.Stride);

                Parallel.For(0, Height, Y =>
                {
                    Copier((IntPtr.Add(bits.Scan0, Y * bits.Stride), Y * rowSize, rowSize));
                });
            }
            finally
            {
                Bitmap.UnlockBits(bits);
            }
        }

        public void CopyTo(byte[] Buffer)
        {
            Copy(Param =>
            {
                var (srcPtr, destOffset, length) = Param;

                Marshal.Copy(srcPtr, Buffer, destOffset, length);
            });
        }

        public void CopyTo(IntPtr Buffer)
        {
            Copy(Param =>
            {
                var (srcPtr, destOffset, length) = Param;

                Kernel32.CopyMemory(IntPtr.Add(Buffer, destOffset), srcPtr, length);
            });
        }
    }
}