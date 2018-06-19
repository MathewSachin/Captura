using System;
using System.IO;

namespace Screna
{
    public interface IBitmapFrame : IDisposable
    {
        void SaveGif(Stream Stream);

        int Width { get; }

        int Height { get; }

        void CopyTo(byte[] Buffer, int Length);

        IBitmapEditor GetEditor();
    }
}