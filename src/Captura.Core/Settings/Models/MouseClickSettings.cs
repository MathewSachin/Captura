using System.Drawing;

namespace Captura
{
    public class MouseClickSettings : PropertyStore
    {
        public bool Display
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int Radius
        {
            get => Get(25);
            set => Set(value);
        }

        public Color Color
        {
            get => Get(Color.Yellow);
            set => Set(value);
        }

        public int BorderThickness
        {
            get => Get<int>();
            set => Set(value);
        }

        public Color BorderColor
        {
            get => Get(Color.Black);
            set => Set(value);
        }
    }
}