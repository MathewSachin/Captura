namespace Captura.FFmpeg
{
    public class FFmpegCodecSettings : PropertyStore
    {
        public string Name
        {
            get => Get("Custom");
            set => Set(value);
        }

        public string Args
        {
            get => Get("-vcodec libx264 -crf 30 -pix_fmt yuv420p -preset ultrafast");
            set => Set(value);
        }

        public string Extension
        {
            get => Get(".mp4");
            set => Set(value);
        }

        public string AudioFormat
        {
            get => Get("Mp3");
            set => Set(value);
        }
    }
}