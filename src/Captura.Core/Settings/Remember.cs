using Captura.Models;
using Captura.Models.VideoItems;
using System.Configuration;

namespace Captura
{
    public partial class Settings
    {
        [UserScopedSetting]
        [DefaultSettingValue("Png")]
        public string LastScreenShotFormat
        {
            get => Get<string>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue(nameof(VideoWriterKind.FFMpeg))]
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
        [DefaultSettingValue(nameof(VideoSourceKind.Region))]
        public VideoSourceKind LastSourceKind
        {
            get => Get<VideoSourceKind>();
            set => Set(value);
        }

        [UserScopedSetting]
        //[DefaultSettingValue("0,0,854,480")]
        [DefaultSettingValue("0,0,940,560")]
        public string LastSourceName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue(nameof(RegionSize.YOUTUBE_940_530))]
        public RegionSize LastSelectedRegionSizeKind
        {
            get => Get<RegionSize>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public int LastSessionNumber
        {
            get => Get<int>();
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

        [UserScopedSetting]
        [DefaultSettingValue("Disk")]
        public string LastScreenShotSaveTo
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}
