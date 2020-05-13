using System.Drawing;

namespace Captura.MouseKeyHook
{
    public class MouseClickSettings : MouseOverlaySettings
    {
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

        public bool DisplayScroll
        {
            get => Get(true);
            set => Set(value);
        }

        public Color ScrollCircleColor
        {
            get => Get(Color.FromArgb(239, 83, 80));
            set => Set(value);
        }

        public Color ScrollArrowColor
        {
            get => Get(Color.FromArgb(33, 33, 33));
            set => Set(value);
        }
    }
}