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
            get => Get(Color.FromArgb(255, 235, 59));
            set => Set(value);
        }

        public Color RightClickColor
        {
            get => Get(Color.FromArgb(3, 169, 244));
            set => Set(value);
        }

        public Color MiddleClickColor
        {
            get => Get(Color.FromArgb(76, 175, 80));
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