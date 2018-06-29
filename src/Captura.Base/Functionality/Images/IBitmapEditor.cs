using System;
using System.Drawing;

namespace Captura
{
    public interface IBitmapEditor : IDisposable
    {
        Graphics Graphics { get; }
    }
}