using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class FullScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public static FullScreenItem Instance { get; } = new FullScreenItem();

        FullScreenItem()
        {
            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }
                
        public override string ToString() => Name;

        public string Name => LanguageManager.Instance.FullScreen;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => P;

            return new RegionProvider(WindowProvider.DesktopRectangle, IncludeCursor);
        }
    }
}