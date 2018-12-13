using System.Linq;
using System.Text.RegularExpressions;
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

        public override bool OnSelect()
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1)
            {
                Set(0);
                return true;
            }

            return PickScreen();
        }

        public override bool Deserialize(string Serialized)
        {
            var screen = ScreenItem.Enumerate()
                .Select(M => M.Screen)
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

            if (index >= ScreenItem.Count)
                return false;

            Set(index);

            return true;
        }
    }
}