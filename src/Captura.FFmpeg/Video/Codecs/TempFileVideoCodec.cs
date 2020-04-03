using Captura.Video;

namespace Captura.FFmpeg
{
    class TempFileVideoCodec : X264VideoCodec
    {
        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            base.Apply(Settings, WriterArgs, OutputArgs);

            OutputArgs.AddArg("f", "mp4")
                .AddArg("-y");
        }
    }
}