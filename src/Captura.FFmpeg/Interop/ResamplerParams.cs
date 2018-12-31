using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public class ResamplerParams
    {
        public int ChannelLayout { get; set; }

        public int SampleRate { get; set; }

        public AVSampleFormat SampleFormat { get; set; }
    }
}