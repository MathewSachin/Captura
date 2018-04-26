using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Captura
{
    public class Settings : PropertyStore
    {
        static Settings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter
                    {
                        AllowIntegerValues = false
                    }
                }
            };
        }

        static string GetPath() => Path.Combine(ServiceProvider.SettingsDir, "Captura.json");

        public bool Load()
        {
            try
            {
                var json = File.ReadAllText(GetPath());

                JsonConvert.PopulateObject(json, this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                var sortedProperties = JObject.FromObject(this).Properties().OrderBy(J => J.Name);

                var jobj = new JObject(sortedProperties.Cast<object>().ToArray());

                File.WriteAllText(GetPath(), jobj.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EnsureOutPath()
        {
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);
        }

        public ProxySettings Proxy { get; } = new ProxySettings();

        public WebcamOverlaySettings WebcamOverlay { get; } = new WebcamOverlaySettings();

        public MouseClickSettings Clicks { get; } = new MouseClickSettings();
        
        public KeystrokesSettings Keystrokes { get; } = new KeystrokesSettings();
        
        public VisualSettings UI { get; } = new VisualSettings();

        public ScreenShotSettings ScreenShots { get; } = new ScreenShotSettings();

        public VideoSettings Video { get; } = new VideoSettings();

        public AudioSettings Audio { get; } = new AudioSettings();

        public FFMpegSettings FFMpeg { get; } = new FFMpegSettings();

        public GifSettings Gif { get; } = new GifSettings();

        public int StartDelay
        {
            get => Get(0);
            set => Set(value);
        }

        public int Duration
        {
            get => Get(0);
            set => Set(value);
        }

        public bool CopyOutPathToClipboard
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        public int RecentMax
        {
            get => Get(30);
            set => Set(value);
        }

        public string OutPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public bool IncludeCursor
        {
            get => Get(true);
            set => Set(value);
        }
    }
}
