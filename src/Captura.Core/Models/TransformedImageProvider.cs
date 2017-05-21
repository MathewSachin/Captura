using Captura.Models;
using Screna;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Captura
{
    public class TransformedImageProvider : IImageProvider
    {
        readonly float _resizeWidth, _resizeHeight;

        readonly IImageProvider _imageSource;
        readonly Color _backgroundColor;
        
        public TransformedImageProvider(IImageProvider ImageSource)
        {
            _imageSource = ImageSource ?? throw new ArgumentNullException(nameof(ImageSource));

            Height = ImageSource.Height;
            Width = ImageSource.Width;

            #region Resize
            if (Settings.Instance.DoResize)
            {
                _backgroundColor = Color.Gray;

                Height = Settings.Instance.ResizeHeight;
                Width = Settings.Instance.ResizeWidth;

                float _originalWidth = ImageSource.Width,
                    _originalHeight = ImageSource.Height;

                var ratio = Math.Min(Width / _originalWidth, Height / _originalHeight);

                _resizeWidth = _originalWidth * ratio;
                _resizeHeight = _originalHeight * ratio;
            }
            #endregion

            #region Rotate
            // Swap Width and Height
            if (Settings.Instance.RotateBy == RotateBy.Rotate90 || Settings.Instance.RotateBy == RotateBy.Rotate270)
            {
                var t = Width;
                Width = Height;
                Height = t;
            }
            #endregion
        }
                
        public Bitmap Capture()
        {
            var bmp = _imageSource.Capture();

            #region Resize
            if (Settings.Instance.DoResize)
            {
                var nonResizedBmp = bmp;
                
                bmp = new Bitmap(Settings.Instance.ResizeWidth, Settings.Instance.ResizeHeight);

                using (var g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    if (_backgroundColor != Color.Transparent)
                        g.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, Settings.Instance.ResizeWidth, Settings.Instance.ResizeHeight);

                    using (nonResizedBmp)
                        g.DrawImage(nonResizedBmp, 0, 0, _resizeWidth, _resizeHeight);
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

            var rotateFlip = (RotateFlipType) Enum.Parse(typeof(RotateFlipType), Settings.Instance.RotateBy.ToString() + flip);

            bmp.RotateFlip(rotateFlip);
            #endregion

            return bmp;
        }

        public int Height { get; }

        public int Width { get; }

        public void Dispose()
        {
            _imageSource.Dispose();
        }

        class CustomImgProvider : IImageProvider
        {
            Bitmap _image;

            public CustomImgProvider(Bitmap Image)
            {
                _image = Image;

                Height = Image.Height;
                Width = Image.Width;
            }

            public int Height { get; }

            public int Width { get; }

            public Bitmap Capture() => _image;

            public void Dispose() { }
        }

        public static Bitmap Transform(Bitmap Image, bool SkipResize = false)
        {
            using (var transformed = new TransformedImageProvider(new CustomImgProvider(Image)))
            {
                var wasResize = Settings.Instance.DoResize;

                if (SkipResize)
                    Settings.Instance.DoResize = false;

                var img = transformed.Capture();
                
                if (SkipResize)
                    Settings.Instance.DoResize = wasResize;

                return img;
            }
        }
    }
}