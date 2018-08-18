using System.Linq;
using SharpDX.DXGI;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeskDuplSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly LanguageManager _loc;
        readonly IVideoSourcePicker _videoSourcePicker;

        public DeskDuplSourceProvider(LanguageManager Loc, IVideoSourcePicker VideoSourcePicker)
        {
            _loc = Loc;
            _videoSourcePicker = VideoSourcePicker;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public void PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            if (screen == null)
                return;

            var outputs = new Factory1()
                .Adapters1
                .SelectMany(M => M.Outputs
                    .Select(N => new
                    {
                        Adapter = M,
                        Output = N.QueryInterface<Output1>()
                    }));

            var match = outputs.FirstOrDefault(M =>
            {
                var r1 = M.Output.Description.DesktopBounds;
                var r2 = screen.Rectangle;

                return r1.Left == r2.Left
                       && r1.Right == r2.Right
                       && r1.Top == r2.Top
                       && r1.Bottom == r2.Bottom;
            });

            if (match != null)
            {
                Source = new DeskDuplItem(match.Adapter, match.Output);
            }
        }

        public IVideoItem Source { get; private set; }

        public string Name => _loc.DesktopDuplication;

        public override string ToString() => Name;

        public string Description =>
            @"Faster API for recording screen as well as fullscreen DirectX games.
Not all games are recordable.
Requires Windows 8 or above.
If it does not work, try running Captura on the Integrated Graphics card.";
    }
}