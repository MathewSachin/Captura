using System.Drawing;

namespace Captura
{
    public class TextOverlaySettings : PositionedOverlaySettings
    {
        bool _display;

        public bool Display
        {
            get => _display;
            set
            {
                _display = value;

                OnPropertyChanged();
            }
        }

        public int FontSize { get; set; } = 20;

        public Color FontColor { get; set; } = Color.White;

        public Color BackgroundColor { get; set; } = Color.Teal;

        public int HorizontalPadding { get; set; } = 15;

        public int VerticalPadding { get; set; } = 15;

        public int CornerRadius { get; set; } = 30;

        public int BorderThickness { get; set; } = 0;

        public Color BorderColor { get; set; } = Color.Black;
    }
}