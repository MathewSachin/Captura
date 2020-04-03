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
    }
}