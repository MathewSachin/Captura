namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FullScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;
        readonly StepsSettings _stepsSettings;

        public FullScreenItem(IPlatformServices PlatformServices, StepsSettings StepsSettings)
        {
            _platformServices = PlatformServices;
            _stepsSettings = StepsSettings;
        }

        public override string ToString() => Name;

        public string Name => null;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return _platformServices.GetAllScreensProvider(IncludeCursor, _stepsSettings.Enabled);
		}
    }
}