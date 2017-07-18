using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Screna;
using System;

namespace Captura.Models
{
    public class ScreenItem : IVideoItem
    {
        public Screen Screen { get; }

        ScreenItem(int i)
        {
            Screen = Screen.AllScreens[i];
        }

        public static int Count => Screen.AllScreens.Length;

        public Bitmap Capture(bool Cursor)
        {
            var rectangle = Screen.Bounds;

            var bmp = new Bitmap(rectangle.Width, rectangle.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rectangle.Location, Point.Empty, rectangle.Size, CopyPixelOperation.SourceCopy);

                if (Cursor)
                    MouseCursor.Draw(g, rectangle.Location);

                g.Flush();
            }

            return bmp;
        }

        public static IEnumerable<ScreenItem> Enumerate()
        {
            var n = Count;

            for (var i = 0; i < n; ++i)
                yield return new ScreenItem(i);
        }

        public override string ToString() => Screen.DeviceName;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point> Offset)
        {
            Offset = () => Screen.Bounds.Location;

            return new ScreenProvider(Screen, IncludeCursor);
        }
    }
}