using System;
using System.Drawing;

namespace Captura
{
    public interface IBitmapLoader : IDisposable
    {
        IDisposable CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride);

        IDisposable LoadBitmap(string FileName, out Size Size);
    }
}