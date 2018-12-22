using System.Drawing;
using System.Drawing.Imaging;
using Captura;
using Captura.Models;

namespace Screna
{
    abstract class EditorBase : IBitmapEditor
    {
        protected readonly Graphics Graphics;

        public void DrawImage(object Image, Rectangle? Region, int Opacity = 100)
        {
            if (!(Image is Image img))
                return;

            var region = Region ?? new Rectangle(Point.Empty, img.Size);

            if (Opacity < 100)
            {
                var colormatrix = new ColorMatrix
                {
                    Matrix33 = Opacity / 100.0f
                };

                var imgAttribute = new ImageAttributes();
                imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Graphics.DrawImage(img, region, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            }
            else Graphics.DrawImage(img, region);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle)
        {
            Graphics.FillRectangle(new SolidBrush(Color), Rectangle);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            Graphics.FillRoundedRectangle(new SolidBrush(Color), Rectangle, CornerRadius);
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            Graphics.FillEllipse(new SolidBrush(Color), Rectangle);
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

        public void DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle)
        {
            Graphics.DrawString(Text, Font, new SolidBrush(Color), LayoutRectangle);
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