namespace Captura
{
    public class ScreenShotSettings : PropertyStore
    {
        public ImageFormats ImageFormat
        {
            get => Get(ImageFormats.Png);
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

        public string ExternalEditor
        {
            get => Get("mspaint");
            set => Set(value);
        }
    }
}