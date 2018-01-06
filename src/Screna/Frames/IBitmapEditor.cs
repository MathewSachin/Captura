using System;
using System.Drawing;

namespace Screna
{
    public interface IBitmapEditor : IDisposable
    {
        Graphics Graphics { get; }
    }
}