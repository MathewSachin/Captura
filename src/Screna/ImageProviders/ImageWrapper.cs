using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Screna
{
    public class ImageWrapper : IDisposable
    {
        ImageWrapper() { }

        public ImageWrapper(int Width, int Height)
        {
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);

            Graphics = Graphics.FromImage(Bitmap);
        }

        public Bitmap Bitmap { get; }

        public Graphics Graphics { get; }

        public BitmapData Lock(ImageLockMode LockMode)
        {
            try { return Bitmap.LockBits(new Rectangle(Point.Empty, Bitmap.Size), LockMode, PixelFormat.Format32bppRgb); }
            catch
            {
                return null;
            }
        }

        public void CopyTo(byte[] Output)
        {
            var bits = Lock(ImageLockMode.ReadOnly);

            if (bits != null)
            {
                Marshal.Copy(bits.Scan0, Output, 0, Output.Length);
                Bitmap.UnlockBits(bits);
            }

            Freed?.Invoke();
        }

        public void Dispose()
        {
            Graphics.Dispose();

            Bitmap.Dispose();
        }

        public event Action Freed;

        public static ImageWrapper Repeat { get; } = new ImageWrapper();
    }
}