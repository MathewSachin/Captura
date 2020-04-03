using System.Collections.Generic;
using Captura.Audio;

namespace Captura.FFmpeg
{
    public class FFmpegAudioItem : IAudioWriterItem
    {
        public FFmpegAudioArgsProvider AudioArgsProvider { get; }

        const string Experimental = "-strict -2";

        FFmpegAudioItem(string Name, string Extension, FFmpegAudioArgsProvider AudioArgsProvider)
        {
            this.Name = Name;
            this.Extension = Extension;

            this.AudioArgsProvider = AudioArgsProvider;
        }

        public string Name { get; }

        string IAudioWriterItem.Name => $"{Name} (FFmpeg)";

        public string Extension { get; }

        public override string ToString() => Name;

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new FFmpegAudioWriter(FileName, AudioQuality, AudioArgsProvider, Wf.SampleRate, Wf.Channels);
        }

        public static FFmpegAudioArgsProvider Aac { get; } = (Quality, OutputArgs) =>
        {
            // bitrate: 32k to 512k (steps of 32k)
            var b = 32 * (1 + (15 * (Quality - 1)) / 99);

            OutputArgs.SetAudioCodec("aac")
                .AddArg(Experimental)
                .AddArg("b:a", $"{b}k");
        };

        public static FFmpegAudioArgsProvider Mp3 { get; } = (Quality, OutputArgs) =>
        {
            // quality: 9 (lowest) to 0 (highest)
            var qscale = (100 - Quality) / 11;

            OutputArgs.SetAudioCodec("libmp3lame")
                .AddArg("qscale:a", qscale);
        };

        public static FFmpegAudioArgsProvider Vorbis { get; } = (Quality, OutputArgs) =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (Quality - 1)) / 99;

            OutputArgs.SetAudioCodec("libvorbis")
                .AddArg("qscale:a", qscale);
        };

        public static FFmpegAudioArgsProvider Opus { get; } = (Quality, OutputArgs) =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (Quality - 1)) / 99;

            OutputArgs.SetAudioCodec("libopus")
                .AddArg("compression_level", qscale);
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
