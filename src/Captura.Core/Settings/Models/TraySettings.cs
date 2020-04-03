using Captura.Hotkeys;

namespace Captura
{
    public class TraySettings : PropertyStore
    {
        public ServiceName LeftClickAction
        {
            get => Get(ServiceName.ShowMainWindow);
            set => Set(value);
        }

        public bool ShowNotifications
        {
            get => Get(true);
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

        public bool MinToTrayOnCaptureStart
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}