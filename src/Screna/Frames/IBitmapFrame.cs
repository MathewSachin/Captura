using System;
using System.Drawing;

namespace Screna
{
    public interface IBitmapFrame : IDisposable
    {
        Bitmap Bitmap { get; }

        IBitmapEditor GetEditor();
    }
}