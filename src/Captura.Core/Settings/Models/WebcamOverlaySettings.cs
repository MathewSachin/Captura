namespace Captura
{
    public class WebcamOverlaySettings : ImageOverlaySettings
    {
        public bool SeparateFile
        {
            get => Get(false);
            set => Set(value);
        }
    }
}