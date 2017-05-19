using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Captura
{
    /// <summary>
    /// Kind of Dependency Injection
    /// </summary>
    public static class ServiceProvider
    {
        static Dictionary<ServiceName, object> _services = new Dictionary<ServiceName, object>();

        /// <summary>
        /// Get the requested Service.
        /// </summary>
        public static T Get<T>(ServiceName ServiceAction)
        {
            return (T)_services[ServiceAction];
        }

        /// <summary>
        /// Gets the Description of a Service.
        /// </summary>
        public static string GetDescription(ServiceName ServiceName)
        {
            switch (ServiceName)
            {
                case ServiceName.Recording:
                    return "Start/Stop Recording";

                case ServiceName.Pause:
                    return "Pause/Resume Recording";

                case ServiceName.ScreenShot:
                    return "ScreenShot";

                case ServiceName.ActiveScreenShot:
                    return "ScreenShot Active Window";

                case ServiceName.DesktopScreenShot:
                    return "ScreenShot Desktop";

                case ServiceName.SelectedWindow:
                    return "Selected Window";

                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Registers a Service.
        /// </summary>
        public static void Register<T>(ServiceName ServiceAction, T Action)
        {
            _services.Add(ServiceAction, Action);
        }

        /// <summary>
        /// Raises the <see cref="HotKeyPressed"/> event with the Hotkey Id.
        /// </summary>
        public static void RaiseHotKeyPressed(int Id)
        {
            HotKeyPressed?.Invoke(Id);
        }

        /// <summary>
        /// Fired (with ID) when a Hotkey is pressed.
        /// </summary>
        public static event Action<int> HotKeyPressed;

        public static bool FFMpegExists
        {
            get
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = FFMpegExePath,
                        Arguments = "-version",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    return true;
                }
                catch { return false; }
            }
        }

        public static string FFMpegExePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Settings.Instance.FFMpegFolder))
                    return "ffmpeg.exe";

                return Path.Combine(Settings.Instance.FFMpegFolder, "ffmpeg.exe");
            }
        }

        public static event Action FFMpegPathChanged;

        public static void RaiseFFMpegPathChanged()
        {
            FFMpegPathChanged?.Invoke();
        }
    }
}
