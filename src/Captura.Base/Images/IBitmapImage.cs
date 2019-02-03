using System;
using System.IO;

namespace Captura
{
    public interface IBitmapImage : IDisposable
    {
        int Width { get; }

        int Height { get; }

        void Save(string FileName, ImageFormats Format);

        void Save(Stream Stream, ImageFormats Format);

        // Assume 32bpp rgba
        //IntPtr Map(bool Read, bool Write);

        //void Unmap();
    }
}