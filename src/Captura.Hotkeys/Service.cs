using System.Text;
using Captura.Loc;

namespace Captura.Hotkeys
{
    public class Service : NotifyPropertyChanged
    {
        readonly ILocalizationProvider _loc;

        public Service(ServiceName ServiceName)
        {
            this.ServiceName = ServiceName;

            _loc = ServiceProvider.Get<ILocalizationProvider>();

            _loc.LanguageChanged += L => RaisePropertyChanged(nameof(Description));
        }

        ServiceName _serviceName;

        public ServiceName ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;

                OnPropertyChanged();

                RaisePropertyChanged(nameof(Description));
            }
        }

        public string Description => GetDescription();

        string GetDescription()
        {
            switch (ServiceName)
            {
                case ServiceName.None:
                    return _loc.None;

                case ServiceName.Recording:
                    return _loc.StartStopRecording;

                case ServiceName.Pause:
                    return _loc.PauseResumeRecording;

                case ServiceName.ScreenShot:
                    return _loc.ScreenShot;

                case ServiceName.ActiveScreenShot:
                    return _loc.ScreenShotActiveWindow;

                case ServiceName.DesktopScreenShot:
                    return _loc.ScreenShotDesktop;

                case ServiceName.ToggleMouseClicks:
                    return _loc.ToggleMouseClicks;

                case ServiceName.ToggleKeystrokes:
                    return _loc.ToggleKeystrokes;

                case ServiceName.ScreenShotRegion:
                    return "Screenshot (Region)";

                case ServiceName.ScreenShotScreen:
                    return "ScreenShot (Screen)";

                case ServiceName.ScreenShotWindow:
                    return "ScreenShot (Window)";

                default:
                    return SpaceAtCapitals(ServiceName);
            }
        }

        static string SpaceAtCapitals<T>(T Obj)
        {
            var s = Obj.ToString();

            var sb = new StringBuilder();

            for (var i = 0; i < s.Length; ++i)
            {
                if (i != 0 && char.IsUpper(s[i]))
                    sb.Append(" ");

                sb.Append(s[i]);
            }

            return sb.ToString();
        }
    }
}