namespace Captura.FFmpeg
{
    class TwitchVideoCodec : StreamingVideoCodec
    {
        public TwitchVideoCodec() : base("Twitch", "Stream to Twitch") { }

        protected override string GetLink(FFmpegSettings Settings)
        {
            return $"rtmp://live.twitch.tv/app/{Settings.TwitchKey}";
        }
    }
}