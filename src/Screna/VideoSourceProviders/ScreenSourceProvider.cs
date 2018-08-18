namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        
        public ScreenSourceProvider(LanguageManager Loc, IVideoSourcePicker VideoSourcePicker) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;
        }

        public void PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            if (screen != null)
                _source = new ScreenItem(screen);
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Screen;

        public override string Description => "Record a specific screen.";
    }
}