using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public abstract class FFmpegAudioCodecInfo : FFmpegCodecInfo
    {
        protected FFmpegAudioCodecInfo(AVCodecID Id, AVSampleFormat SampleFormat) : base(Id)
        {
            this.SampleFormat = SampleFormat;
        }

        protected FFmpegAudioCodecInfo(string Name, AVSampleFormat SampleFormat) : base(Name)
        {
            this.SampleFormat = SampleFormat;
        }

        public AVSampleFormat SampleFormat { get; }

        public abstract void SetOptions(FFmpegAudioStream AudioStream, int Quality);
    }
}