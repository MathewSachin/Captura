using System.Configuration;
using System.Drawing;
// ReSharper disable UnusedMember.Global

namespace Captura
{
    public partial class Settings
    {
        [UserScopedSetting]
        [DefaultSettingValue("#FFFFDB74")]
        public Color MouseClick_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("25")]
        public float MouseClick_Radius
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public float MouseClick_Border
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color MouseClick_BorderColor
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public float Keystrokes_Border
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color Keystrokes_BorderColor
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("White")]
        public Color Keystrokes_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("#FF3384CB")]
        public Color KeystrokesRect_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("20")]
        public int Keystrokes_FontSize
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_MaxLength
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_PaddingX
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_PaddingY
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("30")]
        public int Keystrokes_CornerRadius
        {
            get => Get<int>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.Start))]
        public Alignment Keystrokes_XAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.End))]
        public Alignment Keystrokes_YAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("80")]
        public int Keystrokes_X
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("100")]
        public int Keystrokes_Y
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("2")]
        public int Keystrokes_MaxSeconds
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("4")]
        public int Keystrokes_History_Count
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("10")]
        public int Keystrokes_History_Spacing
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
