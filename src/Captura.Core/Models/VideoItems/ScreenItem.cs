using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Screna;
using System;

namespace Captura.Models
{
    public class ScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public Screen Screen { get; }

        readonly int _index;
        readonly bool _desktopDuplication;

        ScreenItem(int i, bool DesktopDuplication)
        {
            Screen = Screen.AllScreens[i];

            _index = i;
            _desktopDuplication = DesktopDuplication;
        }

        public static int Count => Screen.AllScreens.Length;

        public Bitmap Capture(bool Cursor)
        {
            return ScreenShot.Capture(Screen, Cursor);
        }

        public static IEnumerable<ScreenItem> Enumerate(bool DesktopDuplication)
        {
            var n = Count;

            for (var i = 0; i < n; ++i)
                yield return new ScreenItem(i, DesktopDuplication);
        }

        public string Name => Screen.DeviceName;

        public override string ToString() => Name;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => new Point(P.X - Screen.Bounds.X, P.Y - Screen.Bounds.Y);
            
            if (_desktopDuplication)
                return new DeskDuplImageProvider(_index, new Rectangle(Point.Empty, Screen.Bounds.Size), IncludeCursor);

            return new RegionProvider(Screen.Bounds, IncludeCursor);
        }
    }
}