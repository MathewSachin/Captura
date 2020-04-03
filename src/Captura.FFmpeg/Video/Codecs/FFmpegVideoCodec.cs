using Captura.Video;

namespace Captura.FFmpeg
{
    abstract class FFmpegVideoCodec : IVideoWriterItem
    {
        protected FFmpegVideoCodec(string Name, string Extension, string Description)
        {
            this.Name = Name;
            this.Extension = Extension;
            this.Description = Description;
        }

        public virtual string Name { get; }

        public virtual string Extension { get; }

        public string Description { get; }

        public abstract FFmpegAudioArgsProvider AudioArgsProvider { get; }

        public abstract void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs);

        public virtual IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegWriter(FFmpegVideoWriterArgs.FromVideoWriterArgs(Args, this));
        }

        public override string ToString() => Name;
    }
}
