using System;
using System.Drawing;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Captura
{
    public class TextOverlaySettings : PositionedOverlaySettings
    {
        public bool Display
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string FontFamily
        {
            get => Get("Arial");
            set => Set(value);
        }

        public int FontSize
        {
            get => Get(50);
            set => Set(value);
        }

        public Color FontColor
        {
            get => Get(Color.White);
            set => Set(value);
        }

        public Color BackgroundColor
        {
            get => Get(Color.Teal);
            set => Set(value);
        }

        public int HorizontalPadding
        {
            get => Get(15);
            set => Set(value);
        }

        public int VerticalPadding
        {
            get => Get(15);
            set => Set(value);
        }

        public int CornerRadius
        {
            get => Get(30);
            set => Set(value);
        }

        public int BorderThickness
        {
            get => Get<int>();
            set => Set(value);
        }

        public Color BorderColor
        {
            get => Get(Color.FromArgb(158, 158, 158));
            set => Set(value);
        }

        public override double GetX(double FullWidth, double TextWidth)
        {
            return base.GetX(FullWidth, TextWidth + 2 * HorizontalPadding);
        }

        public override double GetY(double FullHeight, double TextHeight)
        {
            return base.GetY(FullHeight, TextHeight + 2 * VerticalPadding);
        }

        public override int ToSetX(double FullWidth, double TextWidth, double Value)
        {
            return base.ToSetX(FullWidth, TextWidth + 2 * HorizontalPadding, Value);
        }

        public override int ToSetY(double FullHeight, double TextHeight, double Value)
        {
            return base.ToSetY(FullHeight, TextHeight + 2 * VerticalPadding, Value);
        }
    }
}