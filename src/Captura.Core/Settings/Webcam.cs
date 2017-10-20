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
    }
}
