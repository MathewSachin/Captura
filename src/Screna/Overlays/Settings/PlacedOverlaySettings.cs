using System;

namespace Captura
{
    public class PlacedOverlaySettings : PositionedOverlaySettings
    {
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
            return (FullWidth * Width / 100).Clip(0, FullWidth);
        }

        public double GetHeight(double FullHeight)
        {
            return (FullHeight * Height / 100).Clip(0, FullHeight);
        }

        public int ToSetWidth(double FullWidth, double Value)
        {
            return (int)Math.Round(Value / FullWidth * 100).Clip(0, 100);
        }

        public int ToSetHeight(double FullHeight, double Value)
        {
            return (int)Math.Round(Value / FullHeight * 100).Clip(0, 100);
        }

        public double GetX(double FullWidth)
        {
            return base.GetX(FullWidth, GetWidth(FullWidth));
        }

        public double GetY(double FullHeight)
        {
            return base.GetY(FullHeight, GetHeight(FullHeight));
        }

        public int ToSetX(double FullWidth, double Value)
        {
            return base.ToSetX(FullWidth, GetWidth(FullWidth), Value);
        }

        public int ToSetY(double FullHeight, double Value)
        {
            return base.ToSetY(FullHeight, GetHeight(FullHeight), Value);
        }
    }
}