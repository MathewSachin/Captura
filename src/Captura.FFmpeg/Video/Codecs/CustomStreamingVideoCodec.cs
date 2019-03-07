namespace Captura.FFmpeg
{
    class CustomStreamingVideoCodec : StreamingVideoCodec
    {
        public CustomStreamingVideoCodec() : base("Custom", "Stream to custom service") { }

        protected override string GetLink(FFmpegSettings Settings)
        {
            return Settings.CustomStreamingUrl;
        }
    }
}