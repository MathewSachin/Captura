using System;
using System.Drawing;

namespace Captura
{
    public interface IBitmapEditor : IDisposable
    {
        Graphics Graphics { get; }

        float Width { get; }
        float Height { get; }

        void FillRectangle(Brush Brush, RectangleF Rectangle);

        void FillEllipse(Brush Brush, RectangleF Rectangle);

        void DrawEllipse(Pen Pen, RectangleF Rectangle);
    }
}