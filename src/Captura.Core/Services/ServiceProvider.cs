using Captura.Properties;
using Captura.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
            return _services.ContainsKey(ServiceAction) ? (T)_services[ServiceAction] : default(T);
        }

        /// <summary>
        /// Gets the Description of a Service.
        /// </summary>
        public static string GetDescriptionKey(ServiceName ServiceName)
        {
            switch (ServiceName)
            {
                case ServiceName.Recording:
                    return nameof(Resources.StartStopRecording);

                case ServiceName.Pause:
                    return nameof(Resources.PauseResumeRecording);

                case ServiceName.ScreenShot:
                    return nameof(Resources.ScreenShot);

                case ServiceName.ActiveScreenShot:
                    return nameof(Resources.ScreenShotActiveWindow);

                case ServiceName.DesktopScreenShot:
                    return nameof(Resources.ScreenShotDesktop);

                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Registers a Service.
        /// </summary>
        public static void Register<T>(ServiceName ServiceAction, T Action)
        {
            if (ServiceAction == ServiceName.SystemTray)
                SystemTray = (ISystemTray)Action;

            else if (ServiceAction == ServiceName.Message)
                Messenger = (IMessageProvider)Action;

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

        public static void LaunchFile(ProcessStartInfo StartInfo)
        {
            try { Process.Start(StartInfo.FileName); }
            catch (Win32Exception E) when (E.NativeErrorCode == 2)
            {
                Messenger.ShowError($"Could not find file: {StartInfo.FileName}");
            }
            catch (Exception E)
            {
                Messenger.ShowError($"Could not open file: {StartInfo.FileName}\n\n\n{E}");
            }
        }

        public static ISystemTray SystemTray { get; private set; }

        public static IMessageProvider Messenger { get; private set; }
                
        public static bool FileExists(string FileName)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName));
        }
    }
}
