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

        public ScreenShotTransformSettings ScreenShotTransform { get; } = new ScreenShotTransformSettings();

        public LastSettings Last { get; } = new LastSettings();

        public FFMpegSettings FFMpeg { get; } = new FFMpegSettings();

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

        public int VideoQuality
        {
            get => Get(70);
            set => Set(value);
        }

        public int FrameRate
        {
            get => Get(10);
            set => Set(value);
        }

        public string Language
        {
            get => Get("en");
            set => Set(value);
        }
        
        public int AudioQuality
        {
            get => Get(50);
            set => Set(value);
        }
        
        #region Gif
        public bool GifRepeat
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int GifRepeatCount
        {
            get => Get<int>();
            set => Set(value);
        }

        public bool GifVariable
        {
            get => Get(true);
            set => Set(value);
        }
        #endregion
    }
}
