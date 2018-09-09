using System.Collections.Generic;
using System.Drawing;
using Screna;
using System;
using System.Linq;
using Monitor = System.Windows.Forms.Screen;

namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public IScreen Screen { get; }

        public ScreenItem(IScreen Screen)
        {
            this.Screen = Screen;
        }

        public static int Count => Monitor.AllScreens.Length;

        public Bitmap Capture(bool Cursor)
        {
            return ScreenShot.Capture(Screen, Cursor);
        }

        public static IEnumerable<ScreenItem> Enumerate()
        {
            return Monitor.AllScreens.Select(M => new ScreenItem(new ScreenWrapper(M)));
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