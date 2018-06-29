using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Captura.Native;

namespace Screna
{
    /// <summary>
    /// Draws the MouseCursor on an Image
    /// </summary>
    public static class MouseCursor
    {
        const int CursorShowing = 1;
                
        /// <summary>
        /// Gets the Current Mouse Cursor Position.
        /// </summary>
        public static Point CursorPosition
        {
            get
            {
                var p = new Point();
                User32.GetCursorPos(ref p);
                return p;
            }
        }

        // hCursor -> (Icon, Hotspot)
        static readonly Dictionary<IntPtr, Tuple<Bitmap, Point>> Cursors = new Dictionary<IntPtr, Tuple<Bitmap, Point>>();
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="G">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public static void Draw(Graphics G, Func<Point, Point> Transform = null)
        {
            // ReSharper disable once RedundantAssignment
            // ReSharper disable once InlineOutVariableDeclaration
            var cursorInfo = new CursorInfo { cbSize = Marshal.SizeOf<CursorInfo>() };

            if (!User32.GetCursorInfo(out cursorInfo))
                return;

            if (cursorInfo.flags != CursorShowing)
                return;

            Bitmap icon;
            Point hotspot;

            if (Cursors.ContainsKey(cursorInfo.hCursor))
            {
                var tuple = Cursors[cursorInfo.hCursor];

                icon = tuple.Item1;
                hotspot = tuple.Item2;
            }
            else
            {
                var hIcon = User32.CopyIcon(cursorInfo.hCursor);

                if (hIcon == IntPtr.Zero)
                    return;

                if (!User32.GetIconInfo(hIcon, out var icInfo))
                    return;

                icon = Icon.FromHandle(hIcon).ToBitmap();
                hotspot = new Point(icInfo.xHotspot, icInfo.yHotspot);

                Cursors.Add(cursorInfo.hCursor, Tuple.Create(icon, hotspot));

                User32.DestroyIcon(hIcon);

                Gdi32.DeleteObject(icInfo.hbmColor);
                Gdi32.DeleteObject(icInfo.hbmMask);
            }

            var location = new Point(cursorInfo.ptScreenPos.X - hotspot.X,
                cursorInfo.ptScreenPos.Y - hotspot.Y);

            if (Transform != null)
                location = Transform(location);

            try
            {
                G.DrawImage(icon, new Rectangle(location, icon.Size));
            }
            catch (ArgumentException) { }
        }
    }
}