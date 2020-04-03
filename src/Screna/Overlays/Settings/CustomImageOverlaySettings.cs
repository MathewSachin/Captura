namespace Captura.Video
{
    public class CustomImageOverlaySettings : ImageOverlaySettings
    {
        public string Source
        {
            get => Get("");
            set => Set(value);
        }

        public bool Display
        {
            get => Get(true);
            set => Set(value);
        }
    }
}