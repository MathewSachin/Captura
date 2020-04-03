using System;
using System.Globalization;
using System.Runtime.CompilerServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Captura.Loc
{
    public class LanguageFields : NotifyPropertyChanged, ILocalizationProvider
    {
        protected virtual string GetValue(string Key) => "";

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        // ReSharper disable UnusedParameter.Global
        protected virtual void SetValue(string Key, string Value) { }
        // ReSharper restore UnusedParameter.Global

        string Get([CallerMemberName] string PropertyName = null)
        {
            return GetValue(PropertyName);
        }

        void Set(string Value, [CallerMemberName] string PropertyName = null)
        {
            SetValue(PropertyName, Value);

            RaisePropertyChanged(PropertyName);
        }

        public virtual event Action<CultureInfo> LanguageChanged;

        public string About
        {
            get => Get();
            set => Set(value);
        }

        public string AccentColor
        {
            get => Get();
            set => Set(value);
        }

        public string Add
        {
            get => Get();
            set => Set(value);
        }

        public string AlwaysOnTop
        {
            get => Get();
            set => Set(value);
        }

        public string Audio
        {
            get => Get();
            set => Set(value);
        }

        public string AudioFormat
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

        public string ConfigCodecs
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

        public string CrashLogs
        {
            get => Get();
            set => Set(value);
        }

        public string Crop
        {
            get => Get();
            set => Set(value);
        }

        public string CustomSize
        {
            get => Get();
            set => Set(value);
        }

        public string CustomUrl
        {
            get => Get();
            set => Set(value);
        }

        public string DarkTheme
        {
            get => Get();
            set => Set(value);
        }

        public string Delete
        {
            get => Get();
            set => Set(value);
        }

        public string DiscardChanges
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

        public string DownloadFFmpeg
        {
            get => Get();
            set => Set(value);
        }

        public string Edit
        {
            get => Get();
            set => Set(value);
        }

        public string Elapsed
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

        public string FFmpegFolder
        {
            get => Get();
            set => Set(value);
        }

        public string FFmpegLog
        {
            get => Get();
            set => Set(value);
        }

        public string FileMenu
        {
            get => Get();
            set => Set(value);
        }

        public string FileMenuNew
        {
            get => Get();
            set => Set(value);
        }

        public string FileMenuOpen
        {
            get => Get();
            set => Set(value);
        }

        public string FileMenuSave
        {
            get => Get();
            set => Set(value);
        }

        public string FileNaming
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

        public string HideOnFullScreenShot
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

        public string ImageEditor
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

        public string ImageUploadFailed
        {
            get => Get();
            set => Set(value);
        }

        public string ImageUploadSuccess
        {
            get => Get();
            set => Set(value);
        }

        public string ImageUploading
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

        public string Keymap
        {
            get => Get();
            set => Set(value);
        }

        public string Keystrokes
        {
            get => Get();
            set => Set(value);
        }

        public string KeystrokesHistoryCount
        {
            get => Get();
            set => Set(value);
        }

        public string KeystrokesHistorySpacing
        {
            get => Get();
            set => Set(value);
        }

        public string KeystrokesSeparateFile
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

        public string Minimize
        {
            get => Get();
            set => Set(value);
        }

        public string MinToTrayOnCaptureStart
        {
            get => Get();
            set => Set(value);
        }

        public string MinTray
        {
            get => Get();
            set => Set(value);
        }

        public string MinTrayStartup
        {
            get => Get();
            set => Set(value);
        }

        public string MinTrayClose
        {
            get => Get();
            set => Set(value);
        }

        public string MouseClicks
        {
            get => Get();
            set => Set(value);
        }

        public string MouseMiddleClickColor
        {
            get => Get();
            set => Set(value);
        }

        public string MousePointer
        {
            get => Get();
            set => Set(value);
        }

        public string MouseRightClickColor
        {
            get => Get();
            set => Set(value);
        }

        public string NewWindow
        {
            get => Get();
            set => Set(value);
        }

        public string No
        {
            get => Get();
            set => Set(value);
        }

        public string None
        {
            get => Get();
            set => Set(value);
        }

        public string Notifications
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

        public string OpenFromClipboard
        {
            get => Get();
            set => Set(value);
        }

        public string OpenOutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string OutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string Overlays
        {
            get => Get();
            set => Set(value);
        }

        public string Padding
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

        public string PreStartCountdown
        {
            get => Get();
            set => Set(value);
        }

        public string Preview
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

        public string Recent
        {
            get => Get();
            set => Set(value);
        }

        public string RecordStop
        {
            get => Get();
            set => Set(value);
        }

        public string Redo
        {
            get => Get();
            set => Set(value);
        }

        public string Refresh
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

        public string RestoreDefaults
        {
            get => Get();
            set => Set(value);
        }

        public string Right
        {
            get => Get();
            set => Set(value);
        }

        public string SaveToClipboard
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

        public string SelectFFmpegFolder
        {
            get => Get();
            set => Set(value);
        }

        public string SelectOutFolder
        {
            get => Get();
            set => Set(value);
        }

        public string SeparateAudioFiles
        {
            get => Get();
            set => Set(value);
        }

        public string ShowSysNotify
        {
            get => Get();
            set => Set(value);
        }

        public string Sounds
        {
            get => Get();
            set => Set(value);
        }

        public string SnapToWindow
        {
            get => Get();
            set => Set(value);
        }

        public string StartStopRecording
        {
            get => Get();
            set => Set(value);
        }

        public string StreamingKeys
        {
            get => Get();
            set => Set(value);
        }

        public string Timeout
        {
            get => Get();
            set => Set(value);
        }

        public string ToggleMouseClicks
        {
            get => Get();
            set => Set(value);
        }

        public string ToggleKeystrokes
        {
            get => Get();
            set => Set(value);
        }

        public string Tools
        {
            get => Get();
            set => Set(value);
        }

        public string Top
        {
            get => Get();
            set => Set(value);
        }

        public string TrayIcon
        {
            get => Get();
            set => Set(value);
        }

        public string Trim
        {
            get => Get();
            set => Set(value);
        }

        public string Undo
        {
            get => Get();
            set => Set(value);
        }

        public string UploadToImgur
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

        public string ViewCrashLogs
        {
            get => Get();
            set => Set(value);
        }

        public string ViewLicenses
        {
            get => Get();
            set => Set(value);
        }

        public string ViewOnGitHub
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

        public string WebCamSeparateFile
        {
            get => Get();
            set => Set(value);
        }

        public string WebCamView
        {
            get => Get();
            set => Set(value);
        }

        public string Website
        {
            get => Get();
            set => Set(value);
        }

        public string Window
        {
            get => Get();
            set => Set(value);
        }

        public string WindowScreenShotTransparency
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