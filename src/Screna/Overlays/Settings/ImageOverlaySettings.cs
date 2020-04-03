namespace Captura.Video
{
    public class ImageOverlaySettings : PositionedOverlaySettings
    {
        public int Opacity
        {
            get => Get(100);
            set => Set(value);
        }

        public bool Resize
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int ResizeWidth
        {
            get => Get(320);
            set => Set(value);
        }

        public int ResizeHeight
        {
            get => Get(240);
            set => Set(value);
        }
    }
}