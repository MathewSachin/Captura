namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;

        public IScreen Screen { get; }

        public ScreenItem(IScreen Screen, IPlatformServices PlatformServices)
        {
            this.Screen = Screen;
            _platformServices = PlatformServices;
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return _platformServices.GetRegionProvider(Screen.Rectangle, IncludeCursor);
        }
    }
}