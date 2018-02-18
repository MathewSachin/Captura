namespace Captura
{
    public class VideoSettings : PropertyStore
    {
        public string WriterKind
        {
            get => Get("FFMpeg");
            set => Set(value);
        }
        
        public string Writer
        {
            get => Get<string>();
            set => Set(value);
        }
        
        public string SourceKind
        {
            get => Get("Screen");
            set => Set(value);
        }

        public string Source
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Webcam
        {
            get => Get("");
            set => Set(value);
        }

        public int Quality
        {
            get => Get(70);
            set => Set(value);
        }

        public int FrameRate
        {
            get => Get(10);
            set => Set(value);
        }
    }
}