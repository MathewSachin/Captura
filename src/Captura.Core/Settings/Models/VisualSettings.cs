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

        public bool MinimizeOnStart
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool MinToTrayOnStartup
        {
            get => Get(false);
            set => Set(value);
        }

        public bool MinToTrayOnClose
        {
            get => Get(false);
            set => Set(value);
        }

        public bool HideOnFullScreenShot
        {
            get => Get(true);
            set => Set(value);
        }

        public bool TrayNotify
        {
            get => Get(true);
            set => Set(value);
        }

        public bool RegionSelectorDrawingTools
        {
            get => Get(true);
            set => Set(value);
        }

        public string Language
        {
            get => Get("en");
            set => Set(value);
        }

        public ServiceName TrayLeftClickAction
        {
            get => Get(ServiceName.ShowMainWindow);
            set => Set(value);
        }
    }
}