using System.Windows.Forms;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        
        public ScreenSourceProvider(LanguageManager Loc,
            IVideoSourcePicker VideoSourcePicker,
            IIconSet Icons) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;

            Icon = Icons.Screen;
        }

        public bool PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            if (screen == null)
                return false;

            _source = new ScreenItem(screen);
            RaisePropertyChanged(nameof(Source));
            return true;
        }

        public void Set(int Index)
        {
            Set(new ScreenWrapper(Screen.AllScreens[Index]));
        }

        public void Set(IScreen Screen)
        {
            _source = new ScreenItem(Screen);
            RaisePropertyChanged(nameof(Source));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Screen;

        public override string Description { get; } = "Record a specific screen.";

        public override string Icon { get; }
    }
}