using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Input;

namespace Captura
{
    public static class Extensions
    {
        public static void ExecuteIfCan(this ICommand Command)
        {
            if (Command.CanExecute(null))
                Command.Execute(null);
        }

        public static void WriteToClipboard(this string S)
        {
            if (S == null)
                return;

            Clipboard.SetText(S);
        }

        public static Rectangle Even(this Rectangle Rect)
        {
            if (Rect.Width % 2 == 1)
                --Rect.Width;

            if (Rect.Height % 2 == 1)
                --Rect.Height;

            return Rect;
        }

        static GraphicsPath RoundedRect(RectangleF bounds, int radius)
        {
            var diameter = radius * 2;
            var arc = new RectangleF(bounds.Location, new Size(diameter, diameter));
            var path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
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

        public static Bitmap Transform(this Bitmap Image, bool SkipResize = false)
        {
            #region Resize
            if (Settings.Instance.DoResize && !SkipResize)
            {
                var ratio = Math.Min(Settings.Instance.ResizeWidth / Image.Width, Settings.Instance.ResizeHeight / Image.Height);

                var _resizeWidth = Image.Width * ratio;
                var _resizeHeight = Image.Height * ratio;

                var nonResized = Image;

                Image = new Bitmap(Settings.Instance.ResizeWidth, Settings.Instance.ResizeHeight);

                using (var g = Graphics.FromImage(Image))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    var _backgroundColor = Settings.Instance.VideoBackgroundColor;

                    if (_backgroundColor != Color.Transparent)
                        g.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, Settings.Instance.ResizeWidth, Settings.Instance.ResizeHeight);

                    using (nonResized)
                        g.DrawImage(nonResized, 0, 0, _resizeWidth, _resizeHeight);
                }
            }
            #endregion

            #region Rotate Flip
            var flip = "Flip";

            if (!Settings.Instance.FlipHorizontal && !Settings.Instance.FlipVertical)
                flip += "None";

            if (Settings.Instance.FlipHorizontal)
                flip += "X";

            if (Settings.Instance.FlipVertical)
                flip += "Y";

            var rotateFlip = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), Settings.Instance.RotateBy.ToString() + flip);

            Image.RotateFlip(rotateFlip);
            #endregion

            return Image;
        }
    }
}