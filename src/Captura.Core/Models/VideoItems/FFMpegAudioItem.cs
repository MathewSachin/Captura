using System.Collections.Generic;
using Screna.Audio;

namespace Captura.Models
{
    public class FFmpegAudioItem : NoVideoItem
    {
        public FFmpegAudioArgsProvider AudioArgsProvider { get; }

        const string Experimental = "-strict -2";

        // The (FFmpeg) appended to the name is expected in Custom Codecs
        FFmpegAudioItem(string Name, string Extension, FFmpegAudioArgsProvider AudioArgsProvider)
            : base($"{Name} (FFmpeg)", Extension)
        {
            this.AudioArgsProvider = AudioArgsProvider;
        }

        public override IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new FFmpegAudioWriter(FileName, AudioQuality, AudioArgsProvider, Wf.SampleRate, Wf.Channels);
        }

        public static FFmpegAudioArgsProvider Aac { get; } = Quality =>
        {
            // bitrate: 32k to 512k (steps of 32k)
            var b = 32 * (1 + (15 * (Quality - 1)) / 99);

            return $"-c:a aac {Experimental} -b:a {b}k";
        };

        public static FFmpegAudioArgsProvider Mp3 { get; } = Quality =>
        {
            // quality: 9 (lowest) to 0 (highest)
            var qscale = (100 - Quality) / 11;

            return $"-c:a libmp3lame -qscale:a {qscale}";
        };

        public static FFmpegAudioArgsProvider Vorbis { get; } = Quality =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (Quality - 1)) / 99;

            return $"-c:a libvorbis -qscale:a {qscale}";
        };

        public static FFmpegAudioArgsProvider Opus { get; } = Quality =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (Quality - 1)) / 99;

            return $"-c:a libopus -compression_level {qscale}";
        };

        public static IEnumerable<FFmpegAudioItem> Items { get; } = new[]
        {
            new FFmpegAudioItem("AAC", ".aac", Aac),
            new FFmpegAudioItem("Mp3", ".mp3", Mp3),
            new FFmpegAudioItem("Vorbis", ".ogg", Vorbis),
            new FFmpegAudioItem("Opus", ".opus", Opus)
        };
    }
}
