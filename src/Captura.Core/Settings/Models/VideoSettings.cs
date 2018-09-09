namespace Captura
{
    public enum VideoSourceKindEnum
    {
        NoVideo,
        Window,
        Screen,
        FullScreen,
        Region,
        DeskDupl
    }

    public class VideoSettings : PropertyStore
    {
        public string WriterKind
        {
            get => Get("FFmpeg");
            set => Set(value);
        }
        
        public string Writer
        {
            get => Get<string>();
            set => Set(value);
        }
        
        public VideoSourceKindEnum SourceKind
        {
            get => Get(VideoSourceKindEnum.FullScreen);
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
    }
}