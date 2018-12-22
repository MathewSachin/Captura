using System.Drawing;
using System.Drawing.Imaging;
using Captura;
using Captura.Models;

namespace Screna
{
    public class GraphicsEditor : IEditableFrame
    {
        readonly Bitmap _image;
        readonly Graphics _graphics;

        public GraphicsEditor(Bitmap Image)
        {
            _image = Image;

            _graphics = Graphics.FromImage(Image);
        }

        public IBitmapFrame GenerateFrame()
        {
            Dispose();

            return new OneTimeFrame(_image);
        }

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

                _graphics.DrawImage(img, region, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            }
            else _graphics.DrawImage(img, region);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle)
        {
            _graphics.FillRectangle(new SolidBrush(Color), Rectangle);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            _graphics.FillRoundedRectangle(new SolidBrush(Color), Rectangle, CornerRadius);
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            _graphics.FillEllipse(new SolidBrush(Color), Rectangle);
        }

        public void DrawEllipse(Pen Pen, RectangleF Rectangle)
        {
            _graphics.DrawEllipse(Pen, Rectangle);
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle)
        {
            _graphics.DrawRectangle(Pen, Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius)
        {
            _graphics.DrawRoundedRectangle(Pen, Rectangle, CornerRadius);
        }

        public SizeF MeasureString(string Text, Font Font)
        {
            return _graphics.MeasureString(Text, Font);
        }

        public void DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle)
        {
            _graphics.DrawString(Text, Font, new SolidBrush(Color), LayoutRectangle);
        }

        public float Width => _graphics.VisibleClipBounds.Width;

        public float Height => _graphics.VisibleClipBounds.Height;

        public void Dispose()
        {
            _graphics.Dispose();
        }
    }
}