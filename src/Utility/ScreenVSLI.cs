using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Screna;

namespace Captura
{
    class ScreenVSLI
    {
        public Screen Screen { get; }

        ScreenVSLI(int i)
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

                if (Cursor) new MouseCursor().Draw(g, rectangle.Location);

                g.Flush();
            }

            return bmp;
        }

        public static IEnumerable<ScreenVSLI> Enumerate()
        {
            var n = Count;

            for (var i = 0; i < n; ++i)
                yield return new ScreenVSLI(i);
        }

        public override string ToString() => Screen.DeviceName;
    }
}