using Captura.Models;
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
        [DefaultSettingValue(nameof(VideoWriterKind.SharpAvi))]
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
        [DefaultSettingValue(nameof(VideoSourceKind.Screen))]
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
