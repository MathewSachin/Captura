using System.Drawing;
using Captura;
using Captura.Models;

namespace Screna
{
    abstract class EditorBase : IBitmapEditor
    {
        public Graphics Graphics { get; }

        public void FillRectangle(Brush Brush, RectangleF Rectangle)
        {
            Graphics.FillRectangle(Brush, Rectangle);
        }

        public void FillRectangle(Brush Brush, RectangleF Rectangle, int CornerRadius)
        {
            Graphics.FillRoundedRectangle(Brush, Rectangle, CornerRadius);
        }

        public void FillEllipse(Brush Brush, RectangleF Rectangle)
        {
            Graphics.FillEllipse(Brush, Rectangle);
        }

        public void DrawEllipse(Pen Pen, RectangleF Rectangle)
        {
            Graphics.DrawEllipse(Pen, Rectangle);
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle)
        {
            Graphics.DrawRectangle(Pen, Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius)
        {
            Graphics.DrawRoundedRectangle(Pen, Rectangle, CornerRadius);
        }

        public SizeF MeasureString(string Text, Font Font)
        {
            return Graphics.MeasureString(Text, Font);
        }

        public void DrawString(string Text, Font Font, Brush Brush, RectangleF LayoutRectangle)
        {
            Graphics.DrawString(Text, Font, Brush, LayoutRectangle);
        }

        protected EditorBase(Graphics Graphics)
        {
            this.Graphics = Graphics;
        }

        public float Width => Graphics.VisibleClipBounds.Width;

        public float Height => Graphics.VisibleClipBounds.Height;

        public abstract void Dispose();
    }
}