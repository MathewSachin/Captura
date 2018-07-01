using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly LanguageManager _loc;

        public FullScreenItem(LanguageManager Loc)
        {
            _loc = Loc;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }
                
        public override string ToString() => Name;

        public string Name => _loc.FullScreen;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => P;

            return new RegionProvider(WindowProvider.DesktopRectangle, IncludeCursor);
        }
    }
}