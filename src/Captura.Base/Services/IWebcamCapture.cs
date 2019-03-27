using System;
using System.Drawing;

namespace Captura.Models
{
    public interface IWebcamCapture : IDisposable
    {
        IBitmapImage Capture(IBitmapLoader BitmapLoader);

        int Width { get; }

        int Height { get; }

        void UpdatePreview(IWindow Window, Rectangle Location);
    }
}