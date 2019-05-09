namespace Captura
{
    public class CensorOverlaySettings : PlacedOverlaySettings
    {
        public bool Display
        {
            get => Get(false);
            set => Set(value);
        }
    }
}