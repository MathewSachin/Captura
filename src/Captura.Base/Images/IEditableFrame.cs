using System;
using System.Drawing;

namespace Captura
{
    public interface IEditableFrame : IDisposable
    {
        float Width { get; }
        float Height { get; }

        void DrawImage(object Image, Rectangle? Region, int Opacity = 100);

        void FillRectangle(Color Color, RectangleF Rectangle);

        void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius);

        void DrawRectangle(Pen Pen, RectangleF Rectangle);

        void DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius);

        void FillEllipse(Color Color, RectangleF Rectangle);

        void DrawEllipse(Pen Pen, RectangleF Rectangle);

        SizeF MeasureString(string Text, Font Font);

        void DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle);

        IBitmapFrame GenerateFrame();
    }
}