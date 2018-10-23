﻿using System.Text;

namespace Captura.Models
{
    public class Service : NotifyPropertyChanged
    {
        public Service(ServiceName ServiceName)
        {
            this.ServiceName = ServiceName;
        }

        ServiceName _serviceName;

        public ServiceName ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;

                Description = new TextLocalizer(GetDescriptionKey(value));

                OnPropertyChanged();
            }
        }

        TextLocalizer _description;

        public TextLocalizer Description
        {
            get => _description;
            set
            {
                _description = value;
                
                OnPropertyChanged();
            }
        }

        static string GetDescriptionKey(ServiceName ServiceName)
        {
            switch (ServiceName)
            {
                case ServiceName.None:
                    return nameof(LanguageManager.None);

                case ServiceName.Recording:
                    return nameof(LanguageManager.StartStopRecording);

                case ServiceName.Pause:
                    return nameof(LanguageManager.PauseResumeRecording);

                case ServiceName.ScreenShot:
                    return nameof(LanguageManager.ScreenShot);

                case ServiceName.ActiveScreenShot:
                    return nameof(LanguageManager.ScreenShotActiveWindow);

                case ServiceName.DesktopScreenShot:
                    return nameof(LanguageManager.ScreenShotDesktop);

                case ServiceName.ToggleMouseClicks:
                    return nameof(LanguageManager.ToggleMouseClicks);

                case ServiceName.ToggleKeystrokes:
                    return nameof(LanguageManager.ToggleKeystrokes);

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