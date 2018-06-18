using System.Runtime.CompilerServices;

namespace Captura
{
    public class LanguageFields : NotifyPropertyChanged
    {
        protected virtual string GetValue(string Key) => "";

        protected virtual void SetValue(string Key, string Value) { }

        string Get([CallerMemberName] string PropertyName = null)
        {
            return GetValue(PropertyName);
        }

        void Set(string Value, [CallerMemberName] string PropertyName = null)
        {
            SetValue(PropertyName, Value);

            RaisePropertyChanged(PropertyName);
        }

        public string About
        {
            get => Get();
            set => Set(value);
        }

        public string AlwaysOnTop
        {
            get => Get();
            set => Set(value);
        }

        public string AppName
        {
            get => Get();
            set => Set(value);
        }

        public string Audio
        {
            get => Get();
            set => Set(value);
        }

        public string AudioSaved
        {
            get => Get();
            set => Set(value);
        }

        public string BackColor
        {
            get => Get();
            set => Set(value);
        }

        public string BorderColor
        {
            get => Get();
            set => Set(value);
        }

        public string BorderThickness
        {
            get => Get();
            set => Set(value);
        }

        public string Bottom
        {
            get => Get();
            set => Set(value);
        }

        public string CaptureDuration
        {
            get => Get();
            set => Set(value);
        }

        public string Center
        {
            get => Get();
            set => Set(value);
        }

        public string Changelog
        {
            get => Get();
            set => Set(value);
        }

        public string Clear
        {
            get => Get();
            set => Set(value);
        }

        public string ClearRecentList
        {
            get => Get();
            set => Set(value);
        }

        public string Clipboard
        {
            get => Get();
            set => Set(value);
        }

        public string Close
        {
            get => Get();
            set => Set(value);
        }

        public string Color
        {
            get => Get();
            set => Set(value);
        }

        public string Configure
        {
            get => Get();
            set => Set(value);
        }

        public string CopyOutPathClipboard
        {
            get => Get();
            set => Set(value);
        }

        public string CopyPath
        {
            get => Get();
            set => Set(value);
        }

        public string CopyToClipboard
        {
            get => Get();
            set => Set(value);
        }

        public string CornerRadius
        {
            get => Get();
            set => Set(value);
        }

        public string CustomizeClickAndKeysOverlays
        {
            get => Get();
            set => Set(value);
        }

        public string Delete
        {
            get => Get();
            set => Set(value);
        }

        public string DesktopDuplication
        {
            get => Get();
            set => Set(value);
        }

        public string Disk
        {
            get => Get();
            set => Set(value);
        }

        public string Donate
        {
            get => Get();
            set => Set(value);
        }

        public string ErrorOccurred
        {
            get => Get();
            set => Set(value);
        }

        public string Exit
        {
            get => Get();
            set => Set(value);
        }

        public string Extras
        {
            get => Get();
            set => Set(value);
        }

        public string FFMpegFolder
        {
            get => Get();
            set => Set(value);
        }

        public string FFMpegLog
        {
            get => Get();
            set => Set(value);
        }

        public string Flip
        {
            get => Get();
            set => Set(value);
        }

        public string FontSize
        {
            get => Get();
            set => Set(value);
        }

        public string FrameRate
        {
            get => Get();
            set => Set(value);
        }

        public string FullScreen
        {
            get => Get();
            set => Set(value);
        }

        public string Gif
        {
            get => Get();
            set => Set(value);
        }

        public string Help
        {
            get => Get();
            set => Set(value);
        }

        public string HideOnFullScreenShot
        {
            get => Get();
            set => Set(value);
        }

        public string Horizontal
        {
            get => Get();
            set => Set(value);
        }

        public string Host
        {
            get => Get();
            set => Set(value);
        }

        public string Hotkeys
        {
            get => Get();
            set => Set(value);
        }

        public string ImgEmpty
        {
            get => Get();
            set => Set(value);
        }

        public string ImgFormat
        {
            get => Get();
            set => Set(value);
        }

        public string ImgSavedClipboard
        {
            get => Get();
            set => Set(value);
        }

        public string ImgSavedDisk
        {
            get => Get();
            set => Set(value);
        }

        public string Imgur
        {
            get => Get();
            set => Set(value);
        }

        public string ImgurFailed
        {
            get => Get();
            set => Set(value);
        }

        public string ImgurSuccess
        {
            get => Get();
            set => Set(value);
        }

        public string ImgurUploading
        {
            get => Get();
            set => Set(value);
        }

        public string IncludeClicks
        {
            get => Get();
            set => Set(value);
        }

        public string IncludeCursor
        {
            get => Get();
            set => Set(value);
        }

        public string IncludeKeys
        {
            get => Get();
            set => Set(value);
        }

        public string Keystrokes
        {
            get => Get();
            set => Set(value);
        }

        public string Language
        {
            get => Get();
            set => Set(value);
        }

        public string Left
        {
            get => Get();
            set => Set(value);
        }

        public string LoopbackSource
        {
            get => Get();
            set => Set(value);
        }

        public string Main
        {
            get => Get();
            set => Set(value);
        }

        public string MaxRecent
        {
            get => Get();
            set => Set(value);
        }

        public string MaxTextLength
        {
            get => Get();
            set => Set(value);
        }

        public string MicSource
        {
            get => Get();
            set => Set(value);
        }

        public string MinCapture
        {
            get => Get();
            set => Set(value);
        }

        public string Minimize
        {
            get => Get();
            set => Set(value);
        }

        public string MinTray
        {
            get => Get();
            set => Set(value);
        }

        public string MouseClicks
        {
            get => Get();
            set => Set(value);
        }

        public string No
        {
            get => Get();
            set => Set(value);
        }

        public string NoAudio
        {
            get => Get();
            set => Set(value);
        }

        public string NotSaved
        {
            get => Get();
            set => Set(value);
        }

        public string NoWebcam
        {
            get => Get();
            set => Set(value);
        }

        public string Ok
        {
            get => Get();
            set => Set(value);
        }

        public string OnlyAudio
        {
            get => Get();
            set => Set(value);
        }

        public string Opacity
        {
            get => Get();
            set => Set(value);
        }

        public string OpenOutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string Options
        {
            get => Get();
            set => Set(value);
        }

        public string OutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string PaddingX
        {
            get => Get();
            set => Set(value);
        }

        public string PaddingY
        {
            get => Get();
            set => Set(value);
        }

        public string Password
        {
            get => Get();
            set => Set(value);
        }

        public string Paused
        {
            get => Get();
            set => Set(value);
        }

        public string PauseResume
        {
            get => Get();
            set => Set(value);
        }

        public string PauseResumeRecording
        {
            get => Get();
            set => Set(value);
        }

        public string Port
        {
            get => Get();
            set => Set(value);
        }

        public string Print
        {
            get => Get();
            set => Set(value);
        }

        public string Proxy
        {
            get => Get();
            set => Set(value);
        }

        public string Quality
        {
            get => Get();
            set => Set(value);
        }

        public string Radius
        {
            get => Get();
            set => Set(value);
        }

        public string Ready
        {
            get => Get();
            set => Set(value);
        }

        public string Recent
        {
            get => Get();
            set => Set(value);
        }

        public string Recording
        {
            get => Get();
            set => Set(value);
        }

        public string RecordStop
        {
            get => Get();
            set => Set(value);
        }

        public string Refresh
        {
            get => Get();
            set => Set(value);
        }

        public string Refreshed
        {
            get => Get();
            set => Set(value);
        }

        public string Region
        {
            get => Get();
            set => Set(value);
        }

        public string RegionSelector
        {
            get => Get();
            set => Set(value);
        }

        public string RemoveFromList
        {
            get => Get();
            set => Set(value);
        }

        public string Repeat
        {
            get => Get();
            set => Set(value);
        }

        public string Reset
        {
            get => Get();
            set => Set(value);
        }

        public string Resize
        {
            get => Get();
            set => Set(value);
        }

        public string Right
        {
            get => Get();
            set => Set(value);
        }

        public string Rotate
        {
            get => Get();
            set => Set(value);
        }

        public string SaveLocation
        {
            get => Get();
            set => Set(value);
        }

        public string Screen
        {
            get => Get();
            set => Set(value);
        }

        public string ScreenShot
        {
            get => Get();
            set => Set(value);
        }

        public string ScreenShotActiveWindow
        {
            get => Get();
            set => Set(value);
        }

        public string ScreenShotDesktop
        {
            get => Get();
            set => Set(value);
        }

        public string ScreenShotSaved
        {
            get => Get();
            set => Set(value);
        }

        public string ScreenShotTransforms
        {
            get => Get();
            set => Set(value);
        }

        public string SelectFFMpegFolder
        {
            get => Get();
            set => Set(value);
        }

        public string SelectOutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string ShowSysNotify
        {
            get => Get();
            set => Set(value);
        }

        public string Source
        {
            get => Get();
            set => Set(value);
        }

        public string StartDelay
        {
            get => Get();
            set => Set(value);
        }

        public string StartStopRecording
        {
            get => Get();
            set => Set(value);
        }

        public string Stopped
        {
            get => Get();
            set => Set(value);
        }

        public string Timeout
        {
            get => Get();
            set => Set(value);
        }

        public string Top
        {
            get => Get();
            set => Set(value);
        }

        public string Transforms
        {
            get => Get();
            set => Set(value);
        }

        public string UseProxy
        {
            get => Get();
            set => Set(value);
        }

        public string UseProxyAuth
        {
            get => Get();
            set => Set(value);
        }

        public string UserName
        {
            get => Get();
            set => Set(value);
        }

        public string VarFrameRate
        {
            get => Get();
            set => Set(value);
        }

        public string Vertical
        {
            get => Get();
            set => Set(value);
        }

        public string Video
        {
            get => Get();
            set => Set(value);
        }

        public string VideoEncoder
        {
            get => Get();
            set => Set(value);
        }

        public string VideoSaved
        {
            get => Get();
            set => Set(value);
        }

        public string VideoSource
        {
            get => Get();
            set => Set(value);
        }

        public string Waiting
        {
            get => Get();
            set => Set(value);
        }

        public string WantToTranslate
        {
            get => Get();
            set => Set(value);
        }

        public string WebCam
        {
            get => Get();
            set => Set(value);
        }

        public string WebCamView
        {
            get => Get();
            set => Set(value);
        }

        public string Window
        {
            get => Get();
            set => Set(value);
        }

        public string WindowHeight
        {
            get => Get();
            set => Set(value);
        }

        public string WindowWidth
        {
            get => Get();
            set => Set(value);
        }

        public string Yes
        {
            get => Get();
            set => Set(value);
        }
    }
}