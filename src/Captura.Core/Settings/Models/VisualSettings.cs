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

        public int RegionBorderThickness
        {
            get => Get(3);
            set => Set(value);
        }

        public int ScreenShotNotifyTimeout
        {
            get => Get(5000);
            set => Set(value);
        }

        public bool HideRegionSelectorWhenRecording
        {
            get => Get(false);
            set => Set(value);
        }
    }
}