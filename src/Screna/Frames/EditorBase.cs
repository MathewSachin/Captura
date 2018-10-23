using System.Drawing;
using Captura;

namespace Screna
{
    abstract class EditorBase : IBitmapEditor
    {
        public Graphics Graphics { get; }

        public void FillRectangle(Brush Brush, RectangleF Rectangle)
        {
            Graphics.FillRectangle(Brush, Rectangle);
        }

        public void FillEllipse(Brush Brush, RectangleF Rectangle)
        {
            Graphics.FillEllipse(Brush, Rectangle);
        }

        public void DrawEllipse(Pen Pen, RectangleF Rectangle)
        {
            Graphics.DrawEllipse(Pen, Rectangle);
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