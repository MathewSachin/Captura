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
            TranslationSource.Instance.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(Name));
        }
                
        public override string ToString() => Name;

        public string Name => LanguageManager.FullScreen;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => P;

            return new RegionProvider(WindowProvider.DesktopRectangle, IncludeCursor);
        }
    }
}