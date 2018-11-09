namespace Captura
{
    public class ScreenShotSettings : PropertyStore
    {
        public string ImageFormat
        {
            get => Get("Png");
            set => Set(value);
        }

        public string[] SaveTargets
        {
            get => Get(new []{ "Disk" });
            set => Set(value);
        }

        public bool WindowShotTransparent
        {
            get => Get(true);
            set => Set(value);
        }
    }
}