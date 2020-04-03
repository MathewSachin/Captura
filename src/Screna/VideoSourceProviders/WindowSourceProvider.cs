using System;
using System.Linq;
using System.Text.RegularExpressions;
using Captura.Loc;

namespace Captura.Video
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        readonly IRegionProvider _regionProvider;
        readonly IPlatformServices _platformServices;

        public WindowSourceProvider(ILocalizationProvider Loc,
            IVideoSourcePicker VideoSourcePicker,
            IRegionProvider RegionProvider,
            IIconSet Icons,
            IPlatformServices PlatformServices) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;
            _regionProvider = RegionProvider;
            _platformServices = PlatformServices;

            Icon = Icons.Window;
        }

        bool PickWindow()
        {
            var window = _videoSourcePicker.PickWindow(M => M.Handle != _regionProvider.Handle);

            if (window == null)
                return false;

            _source = new WindowItem(window, _platformServices);

            RaisePropertyChanged(nameof(Source));
            return true;
        }

        void Set(IWindow Window)
        {
            _source = new WindowItem(Window, _platformServices);
            RaisePropertyChanged(nameof(Source));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Window;

        public override string Description { get; } = @"Record a specific window.
The video is of the initial size of the window.";

        public override string Icon { get; }

        public override bool OnSelect()
        {
            return PickWindow();
        }

        public override bool Deserialize(string Serialized)
        {
            var window = _platformServices.EnumerateWindows()
                .FirstOrDefault(M => M.Title == Serialized);

            if (window == null)
                return false;

            Set(window);

            return true;
        }

        public override bool ParseCli(string Arg)
        {
            if (!Regex.IsMatch(Arg, @"^win:-?\d+$"))
                return false;

            var handle = new IntPtr(int.Parse(Arg.Substring(4)));

            Set(_platformServices.GetWindow(handle));

            return true;
        }

        public override IBitmapImage Capture(bool IncludeCursor)
        {
            if (Source is WindowItem windowItem)
            {
                return ScreenShot.Capture(windowItem.Window.Rectangle, IncludeCursor);
            }

            return null;
        }
    }
}
