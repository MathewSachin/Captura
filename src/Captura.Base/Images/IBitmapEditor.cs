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

        void FillRectangle(Brush Brush, RectangleF Rectangle, int CornerRadius);

        void DrawRectangle(Pen Pen, RectangleF Rectangle);

        void DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius);

        void FillEllipse(Brush Brush, RectangleF Rectangle);

        void DrawEllipse(Pen Pen, RectangleF Rectangle);

        SizeF MeasureString(string Text, Font Font);

        void DrawString(string Text, Font Font, Brush Brush, RectangleF LayoutRectangle);
    }
}