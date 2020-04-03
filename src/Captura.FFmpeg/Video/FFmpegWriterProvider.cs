using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Captura.Video;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegWriterProvider : IVideoWriterProvider
    {
        public string Name => "FFmpeg";

        readonly FFmpegSettings _settings;

        public FFmpegWriterProvider(FFmpegSettings Settings)
        {
            _settings = Settings;
        }

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new X264VideoCodec();
            yield return new XvidVideoCodec();

            // Hardware
            yield return new QsvHevcVideoCodec();
            yield return NvencVideoCodec.CreateH264();
            yield return NvencVideoCodec.CreateHevc();

            // Custom
            foreach (var item in _settings.CustomCodecs)
            {
                yield return new CustomFFmpegVideoCodec(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public IVideoWriterItem ParseCli(string Cli)
        {
            var ffmpegExists = FFmpegService.FFmpegExists;

            if (ffmpegExists && Regex.IsMatch(Cli, @"^ffmpeg:\d+$"))
            {
                var index = int.Parse(Cli.Substring(7));

                var writers = this.ToArray();

                if (index < writers.Length)
                {
                    return writers[index];
                }
            }

            return null;
        }

        public string Description => @"Use FFmpeg for encoding.
Requires ffmpeg.exe, if not found option for downloading or specifying path is shown.";
    }
}