namespace Captura.Video
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;
        readonly VideoSettings _videoSettings;

        public IScreen Screen { get; }

        public ScreenItem(IScreen Screen,
            IPlatformServices PlatformServices,
            VideoSettings VideoSettings)
        {
            this.Screen = Screen;
            _platformServices = PlatformServices;
            _videoSettings = VideoSettings;
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return _platformServices.GetScreenProvider(Screen, IncludeCursor, _videoSettings.RecorderMode == RecorderMode.Steps);
        }
    }
}