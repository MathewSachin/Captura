using System;
using System.Collections.Generic;
using Ninject;
using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class StreamingItem : FFMpegItem
    {
        readonly FFMpegItem _baseItem;
        readonly Func<string> _linkFunction;

        StreamingItem(string Name, Func<string> LinkFunction, FFMpegItem BaseItem) : base(Name, () => BaseItem.Extension)
        {
            _baseItem = BaseItem;
            _linkFunction = LinkFunction;
        }

        public override IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider,
            int AudioQuality, IAudioProvider AudioProvider)
        {
            return _baseItem.GetVideoFileWriter(_linkFunction(), FrameRate, VideoQuality, ImageProvider, AudioQuality, AudioProvider, "-g 20 -r 10 -f flv");
        }
        
        public static IEnumerable<StreamingItem> StreamingItems { get; } = new[]
        {
            new StreamingItem("Twitch", () =>
            {
                var settings = ServiceProvider.Kernel.Get<Settings>();

                return $"rtmp://live.twitch.tv/app/{settings.TwitchKey}";
            }, x264),
            new StreamingItem("YouTube Live", () =>
            {
                var settings = ServiceProvider.Kernel.Get<Settings>();

                return $"rtmp://a.rtmp.youtube.com/live2/{settings.YouTubeLiveKey}";
            }, x264)
        };
    }
}