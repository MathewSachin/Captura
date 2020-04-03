namespace Captura.Video
{
    public class VideoSettings : PropertyStore
    {
        public string WriterKind
        {
            get => Get("");
            set => Set(value);
        }
        
        public string Writer
        {
            get => Get<string>();
            set => Set(value);
        }

        public bool PostConvert
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string PostWriter
        {
            get => Get<string>();
            set => Set(value);
        }
        
        public string SourceKind
        {
            get => Get("");
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
            get => Get(20);
            set => Set(value);
        }

        public bool FpsLimit
        {
            get => Get(true);
            set
            {
                Set(value);

                if (value && FrameRate > 30)
                {
                    FrameRate = 30;
                }
            }
        }

        public RecorderMode RecorderMode
        {
            get => Get(RecorderMode.Video);
            set => Set(value);
        }

        public int ReplayDuration
        {
            get => Get(20);
            set => Set(value);
        }
    }
}