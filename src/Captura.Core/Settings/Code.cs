using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace Captura
{
    [SettingsProvider(typeof(JsonSettingsProvider))]
    public partial class Settings : ApplicationSettingsBase
    {
        public static Settings Instance { get; } = (Settings)Synchronized(new Settings());

        Settings()
        {
            InitOverlaySettings();
        }

        void InitOverlaySettings()
        {
            if (Keystrokes == null)
            {
                Keystrokes = new KeystrokesSettings();
            }

            if (MouseClicks == null)
            {
                MouseClicks = new MouseClickSettings();
            }

            if (TimeElapsedOverlay == null)
            {
                TimeElapsedOverlay = new TextOverlaySettings
                {
                    HorizontalAlignment = Alignment.End
                };
            }

            if (WebcamOverlay == null)
            {
                WebcamOverlay = new WebcamOverlaySettings
                {
                    HorizontalAlignment = Alignment.End
                };
            }

            if (CustomOverlays == null)
            {
                CustomOverlays = new List<TextOverlaySettings>();
            }
        }

        public void SafeReset()
        {
            Reset();

            InitOverlaySettings();
        }

        T Get<T>([CallerMemberName] string PropertyName = null)
        {
            if (this[PropertyName] is T value)
                return value;

            return default(T);
        }

        void Set<T>(T Value, [CallerMemberName] string PropertyName = null) => this[PropertyName] = Value;
        
        public void EnsureOutPath()
        {
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);
        }
    }
}
