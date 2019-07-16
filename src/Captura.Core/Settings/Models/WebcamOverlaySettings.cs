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

        public int Width
        {
            get => Get(30);
            set => Set(value);
        }

        public int Height
        {
            get => Get(20);
            set => Set(value);
        }

        public int X
        {
            get => Get(80);
            set => Set(value);
        }

        public int Y
        {
            get => Get(80);
            set => Set(value);
        }

        public float GetWidth(float FrameWidth)
        {
            return FrameWidth * Width / 100;
        }

        public float GetHeight(float FrameHeight)
        {
            return FrameHeight * Height / 100;
        }

        public PointF GetPosition(float FrameWidth, float FrameHeight)
        {
            var xLeft = FrameWidth - GetWidth(FrameWidth);
            var yLeft = FrameHeight - GetHeight(FrameHeight);

            var left = xLeft * X / 100;
            var top = yLeft * Y / 100;

            return new PointF(left, top);
        }

    }
}