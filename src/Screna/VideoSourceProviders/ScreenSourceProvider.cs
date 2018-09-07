using System.Windows.Forms;

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

        public bool PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            if (screen == null)
                return false;

            _source = new ScreenItem(screen);
            return true;
        }

        public void Set(int Index)
        {
            _source = new ScreenItem(new ScreenWrapper(Screen.AllScreens[Index]));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Screen;
    }
}