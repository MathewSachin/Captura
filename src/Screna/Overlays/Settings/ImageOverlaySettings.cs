namespace Captura
{
    public class ImageOverlaySettings : PlacedOverlaySettings
    {
        public int Opacity
        {
            get => Get(100);
            set => Set(value);
        }
    }
}