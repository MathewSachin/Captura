using System.Drawing;

namespace Captura
{
    public class MouseClickSettings
    {
        public bool Display { get; set; }

        public int Radius { get; set; } = 25;

        public Color Color { get; set; } = Color.Yellow;

        public int BorderThickness { get; set; } = 0;

        public Color BorderColor { get; set; } = Color.Black;
    }
}