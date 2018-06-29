﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
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

            Marshal.Copy(bits.Scan0, Buffer, 0, Length);

            Bitmap.UnlockBits(bits);
        }

        public abstract IBitmapEditor GetEditor();
    }
}