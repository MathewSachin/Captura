using System.Configuration;

namespace Captura
{
    public partial class Settings
    {
        [UserScopedSetting]
        [DefaultSettingValue(".mp4")]
        public string FFMpeg_CustomExtension
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("-vcodec libx264 -crf 30 -pix_fmt yuv420p -preset ultrafast")]
        public string FFMpeg_CustomArgs
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}
