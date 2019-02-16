using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using Captura.Models;

namespace Captura
{
    public class Settings : PropertyStore
    {
        static Settings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter
                    {
                        AllowIntegerValues = false
                    }
                }
            };
        }

        public Settings(FFmpegSettings FFmpeg)
        {
            this.FFmpeg = FFmpeg;
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

        public ImgurSettings Imgur { get; } = new ImgurSettings();

        public WebcamOverlaySettings WebcamOverlay { get; set; } = new WebcamOverlaySettings();

        public MouseOverlaySettings MousePointerOverlay { get; set; } = new MouseOverlaySettings
        {
            Color = Color.FromArgb(200, 239, 108, 0)
        };

        public MouseClickSettings Clicks { get; set; } = new MouseClickSettings();
        
        public KeystrokesSettings Keystrokes { get; set; } = new KeystrokesSettings();

        public TextOverlaySettings Elapsed { get; set; } = new TextOverlaySettings();

        public ObservableCollection<CensorOverlaySettings> Censored { get; } = new ObservableCollection<CensorOverlaySettings>();
        
        public VisualSettings UI { get; } = new VisualSettings();

        public ScreenShotSettings ScreenShots { get; } = new ScreenShotSettings();

        public VideoSettings Video { get; } = new VideoSettings();

        public AudioSettings Audio { get; } = new AudioSettings();

        public FFmpegSettings FFmpeg { get; }

        public ObservableCollection<CustomOverlaySettings> TextOverlays { get; } = new ObservableCollection<CustomOverlaySettings>();

        public ObservableCollection<CustomImageOverlaySettings> ImageOverlays { get; } = new ObservableCollection<CustomImageOverlaySettings>();

        public SoundSettings Sounds { get; } = new SoundSettings();

        public ImageEditorSettings ImageEditor { get; } = new ImageEditorSettings();

        public TraySettings Tray { get; } = new TraySettings();

        public int PreStartCountdown
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

        public string OutPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public string FilenameFormat
        {
            get => Get("%yyyy%-%MM%-%dd%-%HH%-%mm%-%ss%");
            set => Set(value);
        }

        public string GetFileName(string Extension, string FileName = null)
        {
            if (FileName != null)
                return FileName;

            if (!Extension.StartsWith("."))
                Extension = $".{Extension}";

            if (string.IsNullOrWhiteSpace(FilenameFormat))
                return Path.Combine(OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}{Extension}");

            var now = DateTime.Now;

            var filename = FilenameFormat
                .Replace("%yyyy%", now.ToString("yyyy"))
                .Replace("%yy%", now.ToString("yy"))
                
                .Replace("%MMMM%", now.ToString("MMMM"))
                .Replace("%MMM%", now.ToString("MMM"))
                .Replace("%MM%", now.ToString("MM"))
                
                .Replace("%dd%", now.ToString("dd"))
                .Replace("%ddd%", now.ToString("ddd"))
                .Replace("%dddd%", now.ToString("dddd"))
                
                .Replace("%HH%", now.ToString("HH"))
                .Replace("%hh%", now.ToString("hh"))

                .Replace("%mm%", now.ToString("mm"))
                .Replace("%ss%", now.ToString("ss"))
                .Replace("%tt%", now.ToString("tt"))
                .Replace("%zzz%", now.ToString("zzz"));
            
            var path = Path.Combine(OutPath, $"{filename}{Extension}");

            if (!File.Exists(path))
                return path;

            var i = 1;

            do
            {
                path = Path.Combine(OutPath, $"{filename} ({i++}){Extension}");
            }
            while (File.Exists(path));

            return path;
        }

        public bool IncludeCursor
        {
            get => Get(true);
            set => Set(value);
        }
    }
}
