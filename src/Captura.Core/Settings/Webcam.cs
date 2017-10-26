using System.Configuration;
using System.Drawing;
// ReSharper disable UnusedMember.Global

namespace Captura
{
    public partial class Settings
    {   
        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.End))]
        public Alignment Webcam_XAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue(nameof(Alignment.End))]
        public Alignment Webcam_YAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("20")]
        public int Webcam_X
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("20")]
        public int Webcam_Y
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool Webcam_DoResize
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("320")]
        public int Webcam_ResizeWidth
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("240")]
        public int Webcam_ResizeHeight
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
