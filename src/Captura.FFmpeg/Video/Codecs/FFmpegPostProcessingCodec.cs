using Captura.FFmpeg;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    abstract class FFmpegPostProcessingCodec : FFmpegVideoCodec
    {
        protected FFmpegPostProcessingCodec(string Name, string Extension, string Description)
            : base($"Post Processing: {Name}", Extension, $"{Description}\n{AfterEnc}") { }

        public override IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegPostProcessingWriter(FFmpegVideoWriterArgs.FromVideoWriterArgs(Args, this));
        }

        const string AfterEnc = "Encoding is done after recording has been finished.";
    }
}