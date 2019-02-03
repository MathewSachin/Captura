using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public static class GraphicsExtensions
    {
        static GraphicsPath RoundedRect(RectangleF Bounds, int Radius)
        {
            var path = new GraphicsPath();

            if (Radius == 0)
            {
                path.AddRectangle(Bounds);
                return path;
            }

            var diameter = Radius * 2;
            var arc = new RectangleF(Bounds.Location, new Size(diameter, diameter));

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = Bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = Bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = Bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void DrawRoundedRectangle(this Graphics Graphics, Pen Pen, RectangleF Bounds, int CornerRadius)
        {
            using (var path = RoundedRect(Bounds, CornerRadius))
            {
                Graphics.DrawPath(Pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics Graphics, Brush Brush, RectangleF Bounds, int CornerRadius)
        {
            using (var path = RoundedRect(Bounds, CornerRadius))
            {
                Graphics.FillPath(Brush, path);
            }
        }

        public static ImageFormat ToDrawingImageFormat(this ImageFormats Format)
        {
            switch (Format)
            {
                case ImageFormats.Jpg:
                    return ImageFormat.Jpeg;

                case ImageFormats.Png:
                    return ImageFormat.Png;

                case ImageFormats.Gif:
                    return ImageFormat.Gif;

                case ImageFormats.Bmp:
                    return ImageFormat.Bmp;

                default:
                    return ImageFormat.Png;
            }
        }
    }
}