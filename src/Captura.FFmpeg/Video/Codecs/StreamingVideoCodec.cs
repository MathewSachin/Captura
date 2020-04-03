using Captura.Video;

namespace Captura.FFmpeg
{
    abstract class StreamingVideoCodec : FFmpegVideoCodec
    {
        protected StreamingVideoCodec(string Name, string Description) : base(Name, ".mp4", Description) { }

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            var x264 = new X264VideoCodec();

            x264.Apply(Settings, WriterArgs, OutputArgs);

            OutputArgs.AddArg("g", WriterArgs.FrameRate * 2)
                .AddArg("r", WriterArgs.FrameRate)
                .AddArg("f", "flv");

            var link = GetLink(Settings);

            WriterArgs.FileName = link;
            OutputArgs.UpdateOutput(link);
        }

        protected abstract string GetLink(FFmpegSettings Settings);

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Mp3;
    }
}