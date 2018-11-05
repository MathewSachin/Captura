using System.Drawing;

namespace Captura
{
    public class ImageEditorSettings : PropertyStore
    {
        public Color BrushColor
        {
            get => Get(Color.FromArgb(27, 27, 27));
            set => Set(value);
        }

        public int BrushSize
        {
            get => Get(2);
            set => Set(value);
        }
    }
}