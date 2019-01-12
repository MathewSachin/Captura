using System;
using System.Linq;
using System.Text.RegularExpressions;
using SharpDX.DXGI;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeskDuplSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly IVideoSourcePicker _videoSourcePicker;

        public DeskDuplSourceProvider(LanguageManager Loc,
            IVideoSourcePicker VideoSourcePicker,
            IIconSet Icons)
        {
            _videoSourcePicker = VideoSourcePicker;
            Icon = Icons.Game;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public bool PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            return screen != null && Set(screen);
        }

        public bool SelectFirst()
        {
            var output = new Factory1()
                .Adapters1
                .SelectMany(M => M.Outputs
                    .Select(N => N.QueryInterface<Output1>()))
                    .FirstOrDefault();

            if (output == null)
                return false;

            Source = new DeskDuplItem(output);

            return true;
        }

        public bool Set(IScreen Screen)
        {
            var outputs = new Factory1()
                            .Adapters1
                            .SelectMany(M => M.Outputs
                                .Select(N => N.QueryInterface<Output1>()));

            var match = outputs.FirstOrDefault(M =>
            {
                var r1 = M.Description.DesktopBounds;
                var r2 = Screen.Rectangle;

                return r1.Left == r2.Left
                       && r1.Right == r2.Right
                       && r1.Top == r2.Top
                       && r1.Bottom == r2.Bottom;
            });

            if (match == null)
                return false;

            Source = new DeskDuplItem(match);

            return true;
        }

        IVideoItem _source;

        public IVideoItem Source
        {
            get => _source;
            private set
            {
                _source = value;
                
                OnPropertyChanged();
            }
        }

        public string Name => "Desktop Duplication";

        public string Description { get; } = @"Faster API for recording screen as well as fullscreen DirectX games.
Not all games are recordable.
Requires Windows 8 or above.
If it does not work, try running Captura on the Integrated Graphics card.";

        public string Icon { get; }

        public override string ToString() => Name;

        public bool OnSelect()
        {
            // Select first screen if there is only one
            if (ScreenItem.Count == 1 && SelectFirst())
            {
                return true;
            }

            return PickScreen();
        }

        public void OnUnselect() { }

#pragma warning disable CS0067
        public event Action UnselectRequested;
#pragma warning restore CS0067

        public string Serialize()
        {
            return Source.ToString();
        }

        public bool Deserialize(string Serialized)
        {
            var screen = ScreenItem.Enumerate()
                .Select(M => M.Screen)
                .FirstOrDefault(M => M.DeviceName == Serialized);

            if (screen == null)
                return false;

            Set(screen);

            return true;
        }

        public bool ParseCli(string Arg)
        {
            if (!Regex.IsMatch(Arg, @"^deskdupl:\d+$"))
                return false;

            var index = int.Parse(Arg.Substring(9));

            if (index >= ScreenItem.Count)
                return false;

            Set(ScreenWrapper.Get(index));

            return true;
        }
    }
}