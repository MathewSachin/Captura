namespace Captura.FFmpeg
{
    class YouTubeLiveVideoCodec : StreamingVideoCodec
    {
        public YouTubeLiveVideoCodec() : base("YouTube Live", "Stream to YouTube Live (Not Tested)") { }

        protected override string GetLink(FFmpegSettings Settings)
        {
            return $"rtmp://a.rtmp.youtube.com/live2/{Settings.YouTubeLiveKey}";
        }
    }
}