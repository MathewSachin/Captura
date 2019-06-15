namespace Captura
{
    public class VisualSettings : PropertyStore
    {
        public bool DarkTheme
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool MainWindowTopmost
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string AccentColor
        {
            get => Get<string>();
            set => Set(value);
        }

        public int MainWindowLeft
        {
            get => Get(50);
            set => Set(value);
        }

        public int MainWindowTop
        {
            get => Get(50);
            set => Set(value);
        }

        public bool Expanded
        {
            get => Get(true);
            set => Set(value);
        }

        public bool HideOnFullScreenShot
        {
            get => Get(true);
            set => Set(value);
        }

        public string Language
        {
            get => Get("en");
            set => Set(value);
        }
    }
}