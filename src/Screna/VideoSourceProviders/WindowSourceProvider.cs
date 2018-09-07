using System;
using System.Collections.Generic;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowSourceProvider : VideoSourceProviderBase
    {
        readonly IVideoSourcePicker _videoSourcePicker;

        public WindowSourceProvider(LanguageManager Loc, IVideoSourcePicker VideoSourcePicker) : base(Loc)
        {
            _videoSourcePicker = VideoSourcePicker;
        }

        public bool PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            var window = _videoSourcePicker.PickWindow(SkipWindows);

            if (window == null)
                return false;

            _source = new WindowItem(new Window(window.Handle));
            return true;
        }

        public void Set(IntPtr Handle)
        {
            _source = new WindowItem(new Window(Handle));
        }

        IVideoItem _source;

        public override IVideoItem Source => _source;

        public override string Name => Loc.Window;
    }
}