using System.Drawing;

namespace Captura
{
    public class MouseClickSettings : NotifyPropertyChanged
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

        public int Radius { get; set; } = 25;

        public Color Color { get; set; } = Color.Yellow;

        public int BorderThickness { get; set; } = 0;

        public Color BorderColor { get; set; } = Color.Black;
    }
}