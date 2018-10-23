using System;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        readonly IRegionProvider _regionProvider;

        public WindowSourceProvider(LanguageManager Loc, IVideoSourcePicker VideoSourcePicker, IRegionProvider RegionProvider) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;
            _regionProvider = RegionProvider;
        }

        public bool PickWindow()
        {
            var window = _videoSourcePicker.PickWindow(new [] { _regionProvider.Handle });

            if (window == null)
                return false;

            _source = new WindowItem(new Window(window.Handle));

            RaisePropertyChanged(nameof(Source));
            return true;
        }

        public void Set(IntPtr Handle)
        {
            _source = new WindowItem(new Window(Handle));
            RaisePropertyChanged(nameof(Source));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Window;
    }
}