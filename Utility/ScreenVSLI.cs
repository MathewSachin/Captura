using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Screna;

namespace Captura
{
    class ScreenVSLI : IVideoSourceListItem
    {
        public Screen Screen { get; }

        ScreenVSLI(int i)
        {
            Screen = Screen.AllScreens[i];

            Name = "Screen " + i;
        }

        public static int Count => Screen.AllScreens.Length;

        public Bitmap Capture(bool Cursor)
        {
            var Rectangle = Screen.Bounds;

            var BMP = new Bitmap(Rectangle.Width, Rectangle.Height);

            using (var g = Graphics.FromImage(BMP))
            {
                g.CopyFromScreen(Rectangle.Location, Point.Empty, Rectangle.Size, CopyPixelOperation.SourceCopy);

                if (Cursor) new MouseCursor().Draw(g, Rectangle.Location);

                g.Flush();
            }

            return BMP;
        }

        public static IEnumerable<ScreenVSLI> Enumerate()
        {
            var n = Count;

            for (var i = 0; i < n; ++i)
                yield return new ScreenVSLI(i);
        }

        public string Name { get; }
    }
}