using Screna.Audio;

namespace Captura.Models
{
    public class FFMpegAudioItem : NoVideoItem
    {
        readonly FFMpegAudioArgsProvider _audioArgsProvider;

        const string Experimental = "-strict -2";

        FFMpegAudioItem(string Name, string Extension, FFMpegAudioArgsProvider AudioArgsProvider)
            : base($"{Name} (FFMpeg)", Extension)
        {
            _audioArgsProvider = AudioArgsProvider;
        }

        public override IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new FFMpegAudioWriter(FileName, AudioQuality, _audioArgsProvider, Wf.SampleRate, Wf.Channels);
        }

        public static FFMpegAudioArgsProvider Aac { get; } = q =>
        {
            // bitrate: 32k to 512k (steps of 32k)
            var b = 32 * (1 + (15 * (q - 1)) / 99);

            return $"-c:a aac {Experimental} -b:a {b}k";
        };

        public static FFMpegAudioArgsProvider Mp3 { get; } = q =>
        {
            // quality: 9 (lowest) to 0 (highest)
            var qscale = (100 - q) / 11;

            return $"-c:a libmp3lame -qscale:a {qscale}";
        };

        public static FFMpegAudioArgsProvider Vorbis { get; } = q =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (q - 1)) / 99;

            return $"-c:a libvorbis -qscale:a {qscale}";
        };

        public static FFMpegAudioArgsProvider Opus { get; } = q =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (q - 1)) / 99;

            return $"-c:a libopus -compression_level {qscale}";
        };

        public static FFMpegAudioItem[] Items { get; } = new[]
        {
            new FFMpegAudioItem("AAC", ".aac", Aac),
            new FFMpegAudioItem("Mp3", ".mp3", Mp3),
            new FFMpegAudioItem("Vorbis", ".ogg", Vorbis),
            new FFMpegAudioItem("Opus", ".opus", Opus)
        };
    }
}
