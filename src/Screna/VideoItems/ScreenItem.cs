namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;
        readonly StepsSettings _stepsSettings;

        public IScreen Screen { get; }

        public ScreenItem(IScreen Screen,
            IPlatformServices PlatformServices,
            StepsSettings StepsSettings)
        {
            this.Screen = Screen;
            _platformServices = PlatformServices;
            _stepsSettings = StepsSettings;
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return _platformServices.GetScreenProvider(Screen, IncludeCursor, _stepsSettings.Enabled);
        }
    }
}