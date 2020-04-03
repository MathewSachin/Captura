using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Captura.Video;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StreamingWriterProvider : IVideoWriterProvider
    {
        public string Name => "Stream";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new TwitchVideoCodec();
            yield return new YouTubeLiveVideoCodec();
            yield return new CustomStreamingVideoCodec();
        }

        public static IVideoWriterItem GetCustomStreamingCodec() => new CustomStreamingVideoCodec();

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

        public string Description => @"Stream to streaming sites using FFmpeg (Alpha).
API keys can be set on FFmpeg settings page.";
    }
}