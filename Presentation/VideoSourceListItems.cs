using Screna;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Captura
{
    public interface IVideoSourceListItem { string Name { get; } }

    class WindowVSLI : IVideoSourceListItem
    {
        public IntPtr Handle { get; }

        public static readonly WindowVSLI Desktop = new WindowVSLI(WindowProvider.DesktopHandle, "[Desktop]"),
            TaskBar = new WindowVSLI(WindowProvider.TaskbarHandle, "[TaskBar]");

        public WindowVSLI(IntPtr hWnd)
        {
            Handle = hWnd;
            Name = new WindowHandler(Handle).Title;
        }

        public WindowVSLI(IntPtr hWnd, string Name)
        {
            Handle = hWnd;
            this.Name = Name;
        }

        public string Name { get; }
    }

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