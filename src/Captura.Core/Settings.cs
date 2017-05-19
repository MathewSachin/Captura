using Captura.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Captura
{
    public class Settings : ApplicationSettingsBase
    {
        public static Settings Instance { get; } = (Settings)Synchronized(new Settings());

        Settings() { }

        T Get<T>([CallerMemberName] string PropertyName = null) => (T)this[PropertyName];

        void Set<T>(T Value, [CallerMemberName] string PropertyName = null) => this[PropertyName] = Value;

        #region Remember
        [UserScopedSetting]
        public string LastScreenShotFormat
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("-1, -1")]
        public Point MainWindowLocation
        {
            get => Get<Point>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("None")]
        public VideoWriterKind LastVideoWriterKind
        {
            get => Get<VideoWriterKind>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string LastVideoWriterName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("None")]
        public VideoSourceKind LastSourceKind
        {
            get => Get<VideoSourceKind>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string LastSourceName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string LastAudioWriterName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string LastMicName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string LastSpeakerName
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        [UserScopedSetting]
        [DefaultSettingValue("Disk")]
        public string ScreenShotSaveTo
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public string OutPath
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool IncludeCursor
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MinimizeOnStart
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("70")]
        public int VideoQuality
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("10")]
        public int FrameRate
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MouseClicks
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool KeyStrokes
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool GifUnconstrained
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("en-US")]
        public string Language
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public List<HotkeyModel> Hotkeys
        {
            get => Get<List<HotkeyModel>>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MinimizeToTray
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("50")]
        public int AudioQuality
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool TrayNotify
        {
            get => Get<bool>();
            set => Set(value);
        }

        #region ScreenShot
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool ScreenShotDoResize
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("640")]
        public int ScreenShotResizeWidth
        {
            get => Get<int>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("400")]
        public int ScreenShotResizeHeight
        {
            get => Get<int>();
            set => Set(value);
        }
        #endregion

        #region Gif
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool GifRepeat
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int GifRepeatCount
        {
            get => Get<int>();
            set => Set(value);
        }
        #endregion
    }
}
