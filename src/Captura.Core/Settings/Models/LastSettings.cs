using Captura.Models;

namespace Captura
{
    public class LastSettings : PropertyStore
    {
        public string ScreenShotFormat
        {
            get => Get("Png");
            set => Set(value);
        }
        
        public VideoWriterKind VideoWriterKind
        {
            get => Get(VideoWriterKind.FFMpeg);
            set => Set(value);
        }
        
        public string VideoWriterName
        {
            get => Get<string>();
            set => Set(value);
        }
        
        public VideoSourceKind SourceKind
        {
            get => Get(VideoSourceKind.Screen);
            set => Set(value);
        }

        public string SourceName
        {
            get => Get<string>();
            set => Set(value);
        }

        public string MicName
        {
            get => Get<string>();
            set => Set(value);
        }

        public string SpeakerName
        {
            get => Get<string>();
            set => Set(value);
        }
        
        public string ScreenShotSaveTo
        {
            get => Get("Disk");
            set => Set(value);
        }
    }
}