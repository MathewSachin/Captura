using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public class StreamingItem : FFmpegItem
    {
        readonly FFmpegItem _baseItem;
        readonly Func<string> _linkFunction;

        StreamingItem(string Name, Func<string> LinkFunction, FFmpegItem BaseItem, string Description) : base(Name, () => BaseItem.Extension, Description)
        {
            _baseItem = BaseItem;
            _linkFunction = LinkFunction;
        }

        public override IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            Args.FileName = _linkFunction();

            return _baseItem.GetVideoFileWriter(Args, "-g 20 -r 10 -f flv");
        }

        public static StreamingItem CustomUrl { get; } = new StreamingItem("Custom", () =>
        {
            var settings = ServiceProvider.Get<FFmpegSettings>();

            return settings.CustomStreamingUrl;
        }, x264, "Stream to custom service");

        public static IEnumerable<StreamingItem> StreamingItems { get; } = new[]
        {
            new StreamingItem("Twitch", () =>
            {
                var settings = ServiceProvider.Get<FFmpegSettings>();

                return $"rtmp://live.twitch.tv/app/{settings.TwitchKey}";
            }, x264, "Stream to Twitch"),
            new StreamingItem("YouTube Live", () =>
            {
                var settings = ServiceProvider.Get<FFmpegSettings>();

                return $"rtmp://a.rtmp.youtube.com/live2/{settings.YouTubeLiveKey}";
            }, x264, "Stream to YouTube Live (Not Tested)"),
            CustomUrl
        };
    }
}