using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Captura
{
    public static class LanguageManager
    {
        static readonly JObject DefaultLanguage;
        static JObject _currentLanguage;
        static readonly string LangDir;

        static LanguageManager()
        {
            LangDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Languages");

            DefaultLanguage = LoadLang("en");

            var cultures = new List<CultureInfo>();

            var files  = Directory.EnumerateFiles(LangDir, "*.json");

            foreach (var file in files)
            {
                var cultureName = Path.GetFileNameWithoutExtension(file);

                try
                {
                    var culture = CultureInfo.GetCultureInfo(cultureName);

                    cultures.Add(culture);

                    if (cultureName == Settings.Instance.Language)
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

            cultures.Sort((X, Y) => X.DisplayName.CompareTo(Y.DisplayName));

            AvailableCultures = cultures;
        }

        public static IReadOnlyList<CultureInfo> AvailableCultures { get; }

        static CultureInfo _currentCulture;

        public static CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value;

                Thread.CurrentThread.CurrentUICulture = value;

                Settings.Instance.Language = value.Name;

                _currentLanguage = LoadLang(value.Name);
            }
        }

        static JObject LoadLang(string LanguageId)
        {
            var filePath = Path.Combine(LangDir, $"{LanguageId}.json");

            try
            {
                return JObject.Parse(File.ReadAllText(filePath));
            }
            catch
            {
                return new JObject();
            }
        }

        public static string GetString(string Key)
        {
            if (Key == null)
                return "";

            if (_currentLanguage != null && _currentLanguage.TryGetValue(Key, out var value))
                return value.ToString();

            if (DefaultLanguage.TryGetValue(Key, out value))
                return value.ToString();

            return Key;
        }

        static string Get([CallerMemberName] string PropertyName = null)
        {
            return GetString(PropertyName);
        }

        public static string About => Get();

        public static string AlwaysOnTop => Get();

        public static string AppName => Get();

        public static string Audio => Get();

        public static string AudioSaved => Get();

        public static string BackColor => Get();

        public static string BorderColor => Get();

        public static string BorderThickness => Get();

        public static string Bottom => Get();

        public static string CaptureDuration => Get();

        public static string Center => Get();

        public static string Changelog => Get();

        public static string Clear => Get();

        public static string ClearRecentList => Get();

        public static string Clipboard => Get();

        public static string Close => Get();

        public static string Color => Get();

        public static string Configure => Get();

        public static string CopyOutPathClipboard => Get();

        public static string CopyPath => Get();

        public static string CopyToClipboard => Get();

        public static string CornerRadius => Get();

        public static string CustomizeClickAndKeysOverlays => Get();

        public static string DelayGtDuration => Get();

        public static string Delete => Get();

        public static string Disk => Get();

        public static string Donate => Get();

        public static string ErrorOccured => Get();

        public static string Exit => Get();

        public static string Extras => Get();

        public static string FFMpegFolder => Get();

        public static string FFMpegLog => Get();

        public static string Flip => Get();

        public static string FontSize => Get();

        public static string FrameRate => Get();

        public static string FullScreen => Get();

        public static string Gif => Get();

        public static string Help => Get();

        public static string HideOnFullScreenShot => Get();

        public static string Horizontal => Get();

        public static string Host => Get();

        public static string Hotkeys => Get();

        public static string ImgEmpty => Get();

        public static string ImgFormat => Get();

        public static string ImgSavedClipboard => Get();

        public static string ImgSavedDisk => Get();

        public static string Imgur => Get();

        public static string ImgurFailed => Get();

        public static string ImgurSuccess => Get();

        public static string ImgurUploading => Get();

        public static string IncludeClicks => Get();

        public static string IncludeCursor => Get();

        public static string IncludeKeys => Get();

        public static string Keystrokes => Get();

        public static string Language => Get();

        public static string Left => Get();

        public static string LoopbackSource => Get();

        public static string Main => Get();

        public static string MaxRecent => Get();

        public static string MaxTextLength => Get();

        public static string MicSource => Get();

        public static string MinCapture => Get();

        public static string Minimize => Get();

        public static string MinTray => Get();

        public static string MouseClicks => Get();

        public static string No => Get();

        public static string NoAudio => Get();

        public static string NotSaved => Get();

        public static string NoWebcam => Get();

        public static string Ok => Get();

        public static string OnlyAudio => Get();

        public static string Opacity => Get();

        public static string OpenOutFolder => Get();

        public static string Options => Get();

        public static string OutFolder => Get();

        public static string PaddingX => Get();

        public static string PaddingY => Get();

        public static string Password => Get();

        public static string Paused => Get();

        public static string PauseResume => Get();

        public static string PauseResumeRecording => Get();

        public static string Port => Get();

        public static string Print => Get();

        public static string Proxy => Get();

        public static string Quality => Get();

        public static string Radius => Get();

        public static string Ready => Get();

        public static string Recent => Get();

        public static string Recording => Get();

        public static string RecordStop => Get();

        public static string Refresh => Get();

        public static string Refreshed => Get();

        public static string Region => Get();

        public static string RegionSelector => Get();

        public static string RemoveFromList => Get();

        public static string Repeat => Get();

        public static string Reset => Get();

        public static string Resize => Get();

        public static string Right => Get();

        public static string Rotate => Get();

        public static string SaveLocation => Get();

        public static string Screen => Get();

        public static string ScreenShot => Get();

        public static string ScreenShotActiveWindow => Get();

        public static string ScreenShotDesktop => Get();

        public static string ScreenShotSaved => Get();

        public static string ScreenShotTransforms => Get();

        public static string SelectFFMpegFolder => Get();

        public static string SelectOutFolder => Get();

        public static string ShowSysNotify => Get();

        public static string Source => Get();

        public static string StartDelay => Get();

        public static string StartStopRecording => Get();

        public static string Stopped => Get();

        public static string Timeout => Get();

        public static string Top => Get();

        public static string Transforms => Get();

        public static string UseProxy => Get();

        public static string UseProxyAuth => Get();

        public static string UserName => Get();

        public static string VarFrameRate => Get();

        public static string Vertical => Get();

        public static string Video => Get();

        public static string VideoEncoder => Get();

        public static string VideoSaved => Get();

        public static string VideoSource => Get();

        public static string Waiting => Get();

        public static string WantToTranslate => Get();

        public static string WebCam => Get();

        public static string WebCamView => Get();

        public static string Window => Get();

        public static string WindowHeight => Get();

        public static string WindowWidth => Get();

        public static string Yes => Get();
    }
}
