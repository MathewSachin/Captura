using Screna;

namespace Captura.Models
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

        public string Name => "Around Mouse";

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
