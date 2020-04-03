using System.Linq;
using System.Text.RegularExpressions;
using Captura.Loc;

namespace Captura.Video
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        readonly IPlatformServices _platformServices;
        readonly VideoSettings _videoSettings;

        public ScreenSourceProvider(ILocalizationProvider Loc,
            IVideoSourcePicker VideoSourcePicker,
            IIconSet Icons,
            IPlatformServices PlatformServices,
            VideoSettings VideoSettings) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;
            _platformServices = PlatformServices;
            _videoSettings = VideoSettings;

            Icon = Icons.Screen;
        }

        bool PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            if (screen == null)
                return false;

            _source = new ScreenItem(screen, _platformServices, _videoSettings);
            RaisePropertyChanged(nameof(Source));
            return true;
        }

        void Set(IScreen Screen)
        {
            _source = new ScreenItem(Screen, _platformServices, _videoSettings);
            RaisePropertyChanged(nameof(Source));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Screen;

        public override string Description { get; } = "Record a specific screen.";

        public override string Icon { get; }

        public override bool OnSelect()
        {
            var screens = _platformServices.EnumerateScreens().ToArray();

            // Select first screen if there is only one
            if (screens.Length == 1)
            {
                Set(screens[0]);
                return true;
            }

            return PickScreen();
        }

        public override bool Deserialize(string Serialized)
        {
            var screen = _platformServices.EnumerateScreens()
                .FirstOrDefault(M => M.DeviceName == Serialized);

            if (screen == null)
                return false;

            Set(screen);

            return true;
        }

        public override bool ParseCli(string Arg)
        {
            if (!Regex.IsMatch(Arg, @"^screen:\d+$"))
                return false;

            var index = int.Parse(Arg.Substring(7));

            var screens = _platformServices.EnumerateScreens().ToArray();

            if (index >= screens.Length)
                return false;

            Set(screens[index]);

            return true;
        }

        public override IBitmapImage Capture(bool IncludeCursor)
        {
            if (Source is ScreenItem screenItem)
            {
                return ScreenShot.Capture(screenItem.Screen.Rectangle, IncludeCursor);
            }

            return null;
        }
    }
}