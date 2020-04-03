using System.Drawing;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Captura.Video
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
    }
}