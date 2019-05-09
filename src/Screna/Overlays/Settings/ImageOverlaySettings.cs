using System;

namespace Captura
{
    public class ImageOverlaySettings : PositionedOverlaySettings
    {
        public int Opacity
        {
            get => Get(100);
            set => Set(value);
        }

        public int Width
        {
            get => Get(40);
            set => Set(value);
        }

        public int Height
        {
            get => Get(30);
            set => Set(value);
        }

        public double GetWidth(double FullWidth)
        {
            return FullWidth * Width / 100;
        }

        public double GetHeight(double FullHeight)
        {
            return FullHeight * Height / 100;
        }

        public int SetWidth(double FullWidth, double Value)
        {
            return (int)Math.Round(Value / FullWidth * 100);
        }

        public int SetHeight(double FullHeight, double Value)
        {
            return (int)Math.Round(Value / FullHeight * 100);
        }
    }
}