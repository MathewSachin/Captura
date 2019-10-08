using System;
using System.Drawing;

namespace Captura
{
    public interface IEditableFrame : IBitmapLoader
    {
        float Width { get; }
        float Height { get; }

        void DrawImage(IBitmapImage Image, RectangleF? Region, int Opacity = 100);

        void DrawLine(Point Start, Point End, Color Color, float Width);

        void DrawArrow(Point Start, Point End, Color Color, float Width);

        void FillRectangle(Color Color, RectangleF Rectangle);

        void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius);

        void FillEllipse(Color Color, RectangleF Rectangle);

        void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle);

        IFont GetFont(string FontFamily, int Size);

        SizeF MeasureString(string Text, IFont Font);

        void DrawString(string Text, IFont Font, Color Color, RectangleF LayoutRectangle);

        IBitmapFrame GenerateFrame(TimeSpan Timestamp);
    }
}