using System;
using Screna;
using System.Drawing;
using Captura.Properties;

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

        public string Name => Resources.FullScreen;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => P;

            if (Settings.Instance.UseDeskDupl)
                return new DeskDuplImageProvider(0, WindowProvider.DesktopRectangle, IncludeCursor);

            return new RegionProvider(WindowProvider.DesktopRectangle, IncludeCursor);
        }
    }
}