using Captura.Models;

namespace Captura.FFmpeg
{
    public class FFmpegRollingWriterItem : IVideoWriterItem
    {
        readonly int _duration;

        public FFmpegRollingWriterItem(int Duration)
        {
            _duration = Duration;
        }

        public string Extension => ".mp4";
        public string Description => "Capture Last n seconds";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegRollingWriter(Args, _duration);
        }
    }
}