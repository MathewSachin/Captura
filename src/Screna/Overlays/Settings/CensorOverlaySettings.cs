namespace Captura.Video
{
    public class CensorOverlaySettings : PositionedOverlaySettings
    {
        public bool Display
        {
            get => Get(false);
            set => Set(value);
        }

        public int Width
        {
            get => Get(420);
            set => Set(value);
        }

        public int Height
        {
            get => Get(360);
            set => Set(value);
        }
    }
}