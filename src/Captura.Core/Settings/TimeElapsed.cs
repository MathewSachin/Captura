using System.Configuration;
using System.Drawing;
// ReSharper disable UnusedMember.Global

namespace Captura
{
    public partial class Settings
    {
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool TimeElapsed_Draw
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public float TimeElapsed_Border
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color TimeElapsed_BorderColor
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("White")]
        public Color TimeElapsed_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("#00695c")]
        public Color TimeElapsedRect_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int TimeElapsed_FontSize
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public int TimeElapsed_PaddingX
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public int TimeElapsed_PaddingY
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int TimeElapsed_CornerRadius
        {
            get => Get<int>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.End))]
        public Alignment TimeElapsed_XAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.End))]
        public Alignment TimeElapsed_YAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("80")]
        public int TimeElapsed_X
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("100")]
        public int TimeElapsed_Y
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
