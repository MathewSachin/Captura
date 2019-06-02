using Captura.Models;
using System;

namespace Captura.FFmpeg
{
    public class FFmpegRollingWriterItem : IVideoWriterItem
    {
        readonly int _duration;
        readonly Func<VideoWriterArgs, IVideoFileWriter> _writerGenerator;

        public FFmpegRollingWriterItem(int Duration, Func<VideoWriterArgs, IVideoFileWriter> WriterGenerator)
        {
            _duration = Duration;
            _writerGenerator = WriterGenerator;
        }

        public string Extension => ".mp4";
        public string Description => "Capture Last n seconds";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegRollingWriter(Args, _duration, _writerGenerator);
        }
    }
}