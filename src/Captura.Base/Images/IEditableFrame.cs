using System.Drawing;

namespace Captura
{
    public interface IEditableFrame : IBitmapLoader
    {
        float Width { get; }
        float Height { get; }

        void DrawImage(IBitmapImage Image, Rectangle? Region, int Opacity = 100);

        void FillRectangle(Color Color, RectangleF Rectangle);

        void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius);

        void FillEllipse(Color Color, RectangleF Rectangle);

        void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle);

        IFont GetFont(string FontFamily, int Size);

        SizeF MeasureString(string Text, IFont Font);

        void DrawString(string Text, IFont Font, Color Color, RectangleF LayoutRectangle);

        IBitmapFrame GenerateFrame();
    }
}