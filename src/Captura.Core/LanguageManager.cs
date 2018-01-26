using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Captura
{
    public class LanguageManager : NotifyPropertyChanged
    {
        readonly JObject _defaultLanguage;
        JObject _currentLanguage;
        readonly string _langDir;

        readonly Settings _settings;

        public static LanguageManager Instance { get; } = new LanguageManager();

        public LanguageManager()
        {
            _langDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Languages");

            _defaultLanguage = LoadLang("en");

            var cultures = new List<CultureInfo>();

            var files  = Directory.EnumerateFiles(_langDir, "*.json");

            _settings = ServiceProvider.Get<Settings>();

            foreach (var file in files)
            {
                var cultureName = Path.GetFileNameWithoutExtension(file);

                try
                {
                    if (cultureName == null)
                        continue;

                    var culture = CultureInfo.GetCultureInfo(cultureName);

                    cultures.Add(culture);

                    if (cultureName == _settings.Language)
                    {
                        CurrentCulture = culture;
                    }
                }
                catch
                {
                    // Ignore
                }
            }

            if (_currentCulture == null)
                CurrentCulture = new CultureInfo("en");

            cultures.Sort((X, Y) => string.Compare(X.DisplayName, Y.DisplayName, StringComparison.Ordinal));

            AvailableCultures = cultures;
        }

        public IReadOnlyList<CultureInfo> AvailableCultures { get; }

        CultureInfo _currentCulture;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value;

                Thread.CurrentThread.CurrentUICulture = value;

                _settings.Language = value.Name;

                _currentLanguage = LoadLang(value.Name);

                LanguageChanged?.Invoke(value);

                RaiseAllChanged();
            }
        }

        public event Action<CultureInfo> LanguageChanged;

        JObject LoadLang(string LanguageId)
        {
            var filePath = Path.Combine(_langDir, $"{LanguageId}.json");

            try
            {
                return JObject.Parse(File.ReadAllText(filePath));
            }
            catch
            {
                return new JObject();
            }
        }

        public string this[string Key]
        {
            get
            {
                if (Key == null)
                    return "";

                if (_currentLanguage != null && _currentLanguage.TryGetValue(Key, out var value))
                    return value.ToString();

                if (_defaultLanguage.TryGetValue(Key, out value))
                    return value.ToString();

                return Key;
            }
        }

        string Get([CallerMemberName] string PropertyName = null)
        {
            return this[PropertyName];
        }

        public string About => Get();

        public string AlwaysOnTop => Get();

        public string AppName => Get();

        public string Audio => Get();

        public string AudioSaved => Get();

        public string BackColor => Get();

        public string BorderColor => Get();

        public string BorderThickness => Get();

        public string Bottom => Get();

        public string CaptureDuration => Get();

        public string Center => Get();

        public string Changelog => Get();

        public string Clear => Get();

        public string ClearRecentList => Get();

        public string Clipboard => Get();

        public string Close => Get();

        public string Color => Get();

        public string Configure => Get();

        public string CopyOutPathClipboard => Get();

        public string CopyPath => Get();

        public string CopyToClipboard => Get();

        public string CornerRadius => Get();

        public string CustomizeClickAndKeysOverlays => Get();

        public string DelayGtDuration => Get();

        public string Delete => Get();

        public string DesktopDuplication => Get();

        public string Disk => Get();

        public string Donate => Get();

        public string ErrorOccured => Get();

        public string Exit => Get();

        public string Extras => Get();

        public string FFMpegFolder => Get();

        public string FFMpegLog => Get();

        public string Flip => Get();

        public string FontSize => Get();

        public string FrameRate => Get();

        public string FullScreen => Get();

        public string Gif => Get();

        public string Help => Get();

        public string HideOnFullScreenShot => Get();

        public string Horizontal => Get();

        public string Host => Get();

        public string Hotkeys => Get();

        public string ImgEmpty => Get();

        public string ImgFormat => Get();

        public string ImgSavedClipboard => Get();

        public string ImgSavedDisk => Get();

        public string Imgur => Get();

        public string ImgurFailed => Get();

        public string ImgurSuccess => Get();

        public string ImgurUploading => Get();

        public string IncludeClicks => Get();

        public string IncludeCursor => Get();

        public string IncludeKeys => Get();

        public string Keystrokes => Get();

        public string Language => Get();

        public string Left => Get();

        public string LoopbackSource => Get();

        public string Main => Get();

        public string MaxRecent => Get();

        public string MaxTextLength => Get();

        public string MicSource => Get();

        public string MinCapture => Get();

        public string Minimize => Get();

        public string MinTray => Get();

        public string MouseClicks => Get();

        public string No => Get();

        public string NoAudio => Get();

        public string NotSaved => Get();

        public string NoWebcam => Get();

        public string Ok => Get();

        public string OnlyAudio => Get();

        public string Opacity => Get();

        public string OpenOutFolder => Get();

        public string Options => Get();

        public string OutFolder => Get();

        public string PaddingX => Get();

        public string PaddingY => Get();

        public string Password => Get();

        public string Paused => Get();

        public string PauseResume => Get();

        public string PauseResumeRecording => Get();

        public string Port => Get();

        public string Print => Get();

        public string Proxy => Get();

        public string Quality => Get();

        public string Radius => Get();

        public string Ready => Get();

        public string Recent => Get();

        public string Recording => Get();

        public string RecordStop => Get();

        public string Refresh => Get();

        public string Refreshed => Get();

        public string Region => Get();

        public string RegionSelector => Get();

        public string RemoveFromList => Get();

        public string Repeat => Get();

        public string Reset => Get();

        public string Resize => Get();

        public string Right => Get();

        public string Rotate => Get();

        public string SaveLocation => Get();

        public string Screen => Get();

        public string ScreenShot => Get();

        public string ScreenShotActiveWindow => Get();

        public string ScreenShotDesktop => Get();

        public string ScreenShotSaved => Get();

        public string ScreenShotTransforms => Get();

        public string SelectFFMpegFolder => Get();

        public string SelectOutFolder => Get();

        public string ShowSysNotify => Get();

        public string Source => Get();

        public string StartDelay => Get();

        public string StartStopRecording => Get();

        public string Stopped => Get();

        public string Timeout => Get();

        public string Top => Get();

        public string Transforms => Get();

        public string UseProxy => Get();

        public string UseProxyAuth => Get();

        public string UserName => Get();

        public string VarFrameRate => Get();

        public string Vertical => Get();

        public string Video => Get();

        public string VideoEncoder => Get();

        public string VideoSaved => Get();

        public string VideoSource => Get();

        public string Waiting => Get();

        public string WantToTranslate => Get();

        public string WebCam => Get();

        public string WebCamView => Get();

        public string Window => Get();

        public string WindowHeight => Get();

        public string WindowWidth => Get();

        public string Yes => Get();
    }
}
