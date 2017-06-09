using Captura.Models;
using System.Configuration;

namespace Captura
{
    public partial class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("Png")]
        public string LastScreenShotFormat
        {
            get => Get<string>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("FFMpeg")]
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
        [DefaultSettingValue("Window")]
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
        [DefaultSettingValue("Mp3")]
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
    }
}
