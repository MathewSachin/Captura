namespace Captura.Models
{
    public class FFMpegVideoWriterArgs : VideoWriterArgs
    {
        public static FFMpegVideoWriterArgs FromVideoWriterArgs(VideoWriterArgs Args, FFMpegVideoArgsProvider VideoArgsProvider, FFMpegAudioArgsProvider AudioArgsProvider)
        {
            return new FFMpegVideoWriterArgs
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

        public FFMpegVideoArgsProvider VideoArgsProvider { get; set; }
        public FFMpegAudioArgsProvider AudioArgsProvider { get; set; }
        public int Frequency { get; set; } = 44100;
        public int Channels { get; set; } = 2;
        public string OutputArgs { get; set; } = "";
    }
}