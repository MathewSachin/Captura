// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
using System;

namespace Captura
{
    public class PositionedOverlaySettings : PropertyStore
    {
        public int Left
        {
            get => Get(10);
            set => Set(value);
        }

        public int Top
        {
            get => Get(90);
            set => Set(value);
        }

        public virtual double GetX(double FullWidth, double ItemWidth)
        {
            return ((FullWidth - ItemWidth) * Left / 100).Clip(0, FullWidth);
        }

        public virtual double GetY(double FullHeight, double ItemHeight)
        {
            return ((FullHeight - ItemHeight) * Top / 100).Clip(0, FullHeight);
        }

        public virtual int ToSetX(double FullWidth, double ItemWidth, double Value)
        {
            return (int)Math.Round(Value / (FullWidth - ItemWidth) * 100).Clip(0, 100);
        }

        public virtual int ToSetY(double FullHeight, double ItemHeight, double Value)
        {
            return (int)Math.Round(Value / (FullHeight - ItemHeight) * 100).Clip(0, 100);
        }
    }
}