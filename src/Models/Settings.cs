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

        #region Hotkeys
        [UserScopedSetting]
        [DefaultSettingValue("R")]
        public Keys RecordHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Ctrl, Alt, Shift")]
        public Modifiers RecordHotkeyMod
        {
            get => Get<Modifiers>();
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
        [DefaultSettingValue("Ctrl, Alt, Shift")]
        public Modifiers PauseHotkeyMod
        {
            get => Get<Modifiers>();
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
        [DefaultSettingValue("Ctrl, Alt, Shift")]
        public Modifiers ScreenShotHotkeyMod
        {
            get => Get<Modifiers>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("PrintScreen")]
        public Keys ActiveScreenShotHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Alt")]
        public Modifiers ActiveScreenShotHotkeyMod
        {
            get => Get<Modifiers>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("D")]
        public Keys DesktopHotkey
        {
            get => Get<Keys>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Ctrl, Alt, Shift")]
        public Modifiers DesktopHotkeyMod
        {
            get => Get<Modifiers>();
            set => Set(value);
        }
        #endregion

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
