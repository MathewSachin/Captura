using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Captura
{
    public class Settings : ApplicationSettingsBase
    {
        public static Settings Instance { get; } = (Settings)Synchronized(new Settings());

        Settings() { }

        T Get<T>([CallerMemberName] string PropertyName = null) => (T)this[PropertyName];

        void Set<T>(T Value, [CallerMemberName] string PropertyName = null) => this[PropertyName] = Value;

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
        [DefaultSettingValue("False")]
        public bool UseStereo
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
        [DefaultSettingValue("R")]
        public Keys RecordHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("7")]
        public int RecordHotkeyMod
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("P")]
        public Keys PauseHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("7")]
        public int PauseHotkeyMod
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("S")]
        public Keys ScreenShotHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("7")]
        public int ScreenShotHotkeyMod
        {
            get => Get<int>();
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
    }
}
