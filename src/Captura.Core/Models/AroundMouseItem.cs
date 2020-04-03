namespace Captura.Video
{
    public class AroundMouseItem : IVideoItem
    {
        readonly Settings _settings;
        readonly IPlatformServices _platformServices;

        public AroundMouseItem(Settings Settings, IPlatformServices PlatformServices)
        {
            _settings = Settings;
            _platformServices = PlatformServices;
        }

        public string Name => null;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return new AroundMouseImageProvider(_settings.AroundMouse.Width,
                _settings.AroundMouse.Height,
                _settings.AroundMouse.Margin,
                _platformServices,
                IncludeCursor);
        }
    }
}
