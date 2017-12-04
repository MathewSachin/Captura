using Captura.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Captura.ViewModels;

namespace Captura
{
    /// <summary>
    /// Kind of Dependency Injection
    /// </summary>
    public static class ServiceProvider
    {
        static readonly Dictionary<ServiceName, object> _services = new Dictionary<ServiceName, object>();
        
        static string _settingsDir;

        public static string SettingsDir
        {
            get
            {
                if (_settingsDir == null)
                    _settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Captura");

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);

                return _settingsDir;
            }
            set
            {
                _settingsDir = value;

                if (!Directory.Exists(_settingsDir))
                    Directory.CreateDirectory(_settingsDir);
            }
        }

        public static MainViewModel MainViewModel { get; set; }
        
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
                    return nameof(LanguageManager.StartStopRecording);

                case ServiceName.Pause:
                    return nameof(LanguageManager.PauseResumeRecording);

                case ServiceName.ScreenShot:
                    return nameof(LanguageManager.ScreenShot);

                case ServiceName.ActiveScreenShot:
                    return nameof(LanguageManager.ScreenShotActiveWindow);

                case ServiceName.DesktopScreenShot:
                    return nameof(LanguageManager.ScreenShotDesktop);

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
                
        public static void LaunchFile(ProcessStartInfo StartInfo)
        {
            try { Process.Start(StartInfo.FileName); }
            catch (Win32Exception E) when (E.NativeErrorCode == 2)
            {
                MessageProvider.ShowError($"Could not find file: {StartInfo.FileName}");
            }
            catch (Exception E)
            {
                MessageProvider.ShowError($"Could not open file: {StartInfo.FileName}\n\n\n{E}");
            }
        }

        public static ISystemTray SystemTray { get; set; }

        public static IRegionProvider RegionProvider { get; set; }

        public static IMessageProvider MessageProvider { get; set; }

        public static IWebCamProvider WebCamProvider { get; set; }

        public static IMainWindow MainWindow { get; set; }
                
        public static bool FileExists(string FileName)
        {
            return File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName));
        }
    }
}
