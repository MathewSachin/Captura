using Captura.FFmpeg;

namespace Captura.Models
{
    class FFmpegVideoWriterArgs : VideoWriterArgs
    {
        public static FFmpegVideoWriterArgs FromVideoWriterArgs(VideoWriterArgs Args, FFmpegVideoCodec VideoCodec, FFmpegAudioArgsProvider AudioArgsProvider)
        {
            return new FFmpegVideoWriterArgs
            {
                FileName = Args.FileName,
                ImageProvider = Args.ImageProvider,
                FrameRate = Args.FrameRate,
                VideoQuality = Args.VideoQuality,
                VideoCodec = VideoCodec,
                AudioQuality = Args.AudioQuality,
                AudioArgsProvider = AudioArgsProvider,
                AudioProvider = Args.AudioProvider
            };
        }

        public FFmpegVideoCodec VideoCodec { get; set; }
        public FFmpegAudioArgsProvider AudioArgsProvider { get; set; }
        public int Frequency { get; set; } = 44100;
        public int Channels { get; set; } = 2;
    }
}