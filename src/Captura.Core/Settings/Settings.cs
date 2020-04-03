using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using Captura.Audio;
using Captura.FFmpeg;
using Captura.Imgur;
using Captura.MouseKeyHook;
using Captura.Video;
using Captura.Windows;

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

        public Settings(FFmpegSettings FFmpeg, WindowsSettings WindowsSettings)
        {
            this.FFmpeg = FFmpeg;
            this.WindowsSettings = WindowsSettings;
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

        public TraySettings Tray { get; } = new TraySettings();

        public StepsSettings Steps { get; } = new StepsSettings();

        public AroundMouseSettings AroundMouse { get; } = new AroundMouseSettings();

        public WindowsSettings WindowsSettings { get; }

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

        public string GetOutputPath()
        {
            var path = OutPath;

            string DefaultOutDir() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Captura));

            // If Output Dircetory is not set, fallback to default
            path = string.IsNullOrWhiteSpace(path)
                ? DefaultOutDir()
                : path.Replace(ServiceProvider.CapturaPathConstant, ServiceProvider.AppDir);

            // If drive is not present, fallback to default
            if (!Directory.Exists(Path.GetPathRoot(path)))
            {
                path = DefaultOutDir();
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public string FilenameFormat
        {
            get => Get("%yyyy%-%MM%-%dd%/%HH%-%mm%-%ss%");
            set => Set(value);
        }

        public string GetFileName(string Extension, string FileName = null)
        {
            if (FileName != null)
                return FileName;

            if (!Extension.StartsWith("."))
                Extension = $".{Extension}";

            var outPath = GetOutputPath();

            if (string.IsNullOrWhiteSpace(FilenameFormat))
                return Path.Combine(outPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}{Extension}");

            var now = DateTime.Now;

            var filename = FilenameFormat
                .Replace("%computer%", Environment.MachineName)
                .Replace("%user%", Environment.UserName)

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
            
            var path = Path.Combine(outPath, $"{filename}{Extension}");

            var baseDir = Path.GetDirectoryName(path);
            if (baseDir != null) 
                Directory.CreateDirectory(baseDir);

            if (!File.Exists(path))
                return path;

            var i = 1;

            do
            {
                path = Path.Combine(outPath, $"{filename} ({i++}){Extension}");
            }
            while (File.Exists(path));

            return path;
        }

        public bool IncludeCursor
        {
            get => Get(true);
            set => Set(value);
        }

        public bool RegionPickerHotkeyAutoStartRecording
        {
            get => Get(true);
            set => Set(value);
        }
    }
}
