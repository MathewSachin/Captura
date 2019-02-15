namespace Captura.Models
{
    class FFmpegVideoWriterArgs : VideoWriterArgs
    {
        public static FFmpegVideoWriterArgs FromVideoWriterArgs(VideoWriterArgs Args, FFmpegVideoArgsProvider VideoArgsProvider, FFmpegAudioArgsProvider AudioArgsProvider)
        {
            return new FFmpegVideoWriterArgs
            {
                FileName = Args.FileName,
                ImageProvider = Args.ImageProvider,
                FrameRate = Args.FrameRate,
                VideoQuality = Args.VideoQuality,
                VideoArgsProvider = VideoArgsProvider,
                AudioQuality = Args.AudioQuality,
                AudioArgsProvider = AudioArgsProvider,
                AudioProvider = Args.AudioProvider
            };
        }

        public FFmpegVideoArgsProvider VideoArgsProvider { get; set; }
        public FFmpegAudioArgsProvider AudioArgsProvider { get; set; }
        public int Frequency { get; set; } = 44100;
        public int Channels { get; set; } = 2;
        public string OutputArgs { get; set; } = "";
    }
}