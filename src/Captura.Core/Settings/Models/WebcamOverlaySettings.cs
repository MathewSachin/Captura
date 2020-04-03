using System;
using System.Drawing;

namespace Captura
{
    public class WebcamOverlaySettings : PropertyStore
    {
        public bool SeparateFile
        {
            get => Get(false);
            set => Set(value);
        }

        public int Opacity
        {
            get => Get(100);
            set => Set(value);
        }

        public double Scale
        {
            get => Get(0.3);
            set => Set(value);
        }

        public double XLoc
        {
            get => Get(1.0);
            set => Set(value);
        }

        public double YLoc
        {
            get => Get(1.0);
            set => Set(value);
        }

        public Size GetSize(Size FrameSize, Size WebcamSize)
        {
            if (WebcamSize.Width == 0 || WebcamSize.Height == 0)
            {
                return Size.Empty;
            }

            var wByW = FrameSize.Width / (double)WebcamSize.Width;
            var hByH = FrameSize.Height / (double)WebcamSize.Height;

            if (wByW < hByH)
            {
                var w = (int)Math.Round(FrameSize.Width * Scale);
                var scale = w / (double)WebcamSize.Width;
                var h = (int)Math.Round(WebcamSize.Height * scale);

                return new Size(w, h);
            }
            else
            {
                var h = (int)Math.Round(FrameSize.Height * Scale);
                var scale = h / (double)WebcamSize.Height;
                var w = (int)Math.Round(WebcamSize.Width * scale);

                return new Size(w, h);
            }
        }

        public Point GetPosition(Size FrameSize, Size WebcamSize)
        {
            var size = GetSize(FrameSize, WebcamSize);

            var xLeft = FrameSize.Width - size.Width;
            var yLeft = FrameSize.Height - size.Height;

            var left = (int)Math.Round(xLeft * XLoc);
            var top = (int)Math.Round(yLeft * YLoc);

            return new Point(left, top);
        }
    }
}