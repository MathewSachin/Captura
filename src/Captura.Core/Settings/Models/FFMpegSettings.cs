namespace Captura
{
    public class FFMpegSettings : PropertyStore
    {
        public string FolderPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public string TwitchKey
        {
            get => Get("");
            set => Set(value);
        }

        public string YouTubeLiveKey
        {
            get => Get("");
            set => Set(value);
        }

        public string CustomExtension
        {
            get => Get(".mp4");
            set => Set(value);
        }

        public string CustomArgs
        {
            get => Get("-vcodec libx264 -crf 30 -pix_fmt yuv420p -preset ultrafast");
            set => Set(value);
        }

        public string CustomStreamingUrl
        {
            get => Get("rtmp://");
            set => Set(value);
        }
    }
}