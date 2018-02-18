using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
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

            try { Clipboard.SetText(S); }
            catch (ExternalException)
            {
                ServiceProvider.MessageProvider?.ShowError($"Copy to Clipboard failed:\n\n{S}");
            }
        }
        
        static GraphicsPath RoundedRect(RectangleF Bounds, int Radius)
        {
            var diameter = Radius * 2;
            var arc = new RectangleF(Bounds.Location, new Size(diameter, diameter));
            var path = new GraphicsPath();

            if (Radius == 0)
            {
                path.AddRectangle(Bounds);
                return path;
            }

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

        public static Bitmap Resize(this Bitmap Image, Size Resize, bool KeepAspectRatio)
        {
            var resizeWidth = Resize.Width;
            var resizeHeight = Resize.Height;

            if (KeepAspectRatio)
            {
                var ratio = Math.Min((double) Resize.Width / Image.Width, (double) Resize.Height / Image.Height);

                resizeWidth = (int)(Image.Width * ratio);
                resizeHeight = (int)(Image.Height * ratio);
            }

            var resized = new Bitmap(Resize.Width, Resize.Height);

            using (var g = Graphics.FromImage(resized))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                
                using (Image)
                    g.DrawImage(Image, 0, 0, resizeWidth, resizeHeight);
            }

            return resized;
        }

        public static Bitmap Transform(this Bitmap Image, ScreenShotSettings TransformSettings, bool SkipResize = false)
        {
            if (TransformSettings.Resize && !SkipResize)
            {
                Image = Image.Resize(new Size(TransformSettings.ResizeWidth, TransformSettings.ResizeHeight), true);
            }

            #region Rotate Flip
            var flip = "Flip";

            if (!TransformSettings.FlipHorizontal && !TransformSettings.FlipVertical)
                flip += "None";

            if (TransformSettings.FlipHorizontal)
                flip += "X";

            if (TransformSettings.FlipVertical)
                flip += "Y";

            var rotateFlip = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), TransformSettings.RotateBy + flip);

            Image.RotateFlip(rotateFlip);
            #endregion

            return Image;
        }
    }
}