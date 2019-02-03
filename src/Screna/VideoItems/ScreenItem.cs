using System.Drawing;
using Screna;
using System;

namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public IScreen Screen { get; }

        public ScreenItem(IScreen Screen)
        {
            this.Screen = Screen;
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => new Point(P.X - Screen.Rectangle.X, P.Y - Screen.Rectangle.Y);

            return new RegionProvider(Screen.Rectangle, IncludeCursor);
        }
    }
}