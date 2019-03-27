using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Captura;

namespace Screna
{
    /// <summary>
    /// Draws the MouseCursor on an Image
    /// </summary>
    static class MouseCursor
    {
        const int CursorShowing = 1;

        // hCursor -> (Icon, Hotspot)
        static readonly Dictionary<IntPtr, Tuple<Bitmap, Point>> Cursors = new Dictionary<IntPtr, Tuple<Bitmap, Point>>();
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="G">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public static void Draw(Graphics G, Func<Point, Point> Transform = null)
        {
            GetIcon(Transform, out var icon, out var location);

            if (icon == null)
                return;

            try
            {
                G.DrawImage(icon, new Rectangle(location, icon.Size));
            }
            catch (ArgumentException) { }
        }

        public static void Draw(IEditableFrame G, Func<Point, Point> Transform = null)
        {
            GetIcon(Transform, out var icon, out var location);

            if (icon == null)
                return;

            try
            {
                G.DrawImage(new DrawingImage(icon), new Rectangle(location, icon.Size));
            }
            catch (ArgumentException) { }
        }

        static void GetIcon(Func<Point, Point> Transform, out Bitmap Icon, out Point Location)
        {
            Icon = null;
            Location = Point.Empty;

            // ReSharper disable once RedundantAssignment
            // ReSharper disable once InlineOutVariableDeclaration
            var cursorInfo = new CursorInfo {cbSize = Marshal.SizeOf<CursorInfo>()};

            if (!User32.GetCursorInfo(out cursorInfo))
                return;

            if (cursorInfo.flags != CursorShowing)
                return;

            Point hotspot;

            if (Cursors.ContainsKey(cursorInfo.hCursor))
            {
                var tuple = Cursors[cursorInfo.hCursor];

                Icon = tuple.Item1;
                hotspot = tuple.Item2;
            }
            else
            {
                var hIcon = User32.CopyIcon(cursorInfo.hCursor);

                if (hIcon == IntPtr.Zero)
                    return;

                if (!User32.GetIconInfo(hIcon, out var icInfo))
                    return;

                Icon = System.Drawing.Icon.FromHandle(hIcon).ToBitmap();
                hotspot = new Point(icInfo.xHotspot, icInfo.yHotspot);

                Cursors.Add(cursorInfo.hCursor, Tuple.Create(Icon, hotspot));

                User32.DestroyIcon(hIcon);

                Gdi32.DeleteObject(icInfo.hbmColor);
                Gdi32.DeleteObject(icInfo.hbmMask);
            }

            Location = new Point(cursorInfo.ptScreenPos.X - hotspot.X,
                cursorInfo.ptScreenPos.Y - hotspot.Y);

            if (Transform != null)
                Location = Transform(Location);
        }
    }
}