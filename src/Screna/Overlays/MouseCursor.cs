using Screna.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Screna
{
    /// <summary>
    /// Draws the MouseCursor on an Image
    /// </summary>
    public static class MouseCursor
    {
        #region PInvoke
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport(DllName)]
        static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport(DllName)]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr HObject);
        #endregion
        
        const int CursorShowing = 1;
                
        /// <summary>
        /// Gets the Current Mouse Cursor Position.
        /// </summary>
        public static Point CursorPosition
        {
            get
            {
                var p = new Point();
                GetCursorPos(ref p);
                return p;
            }
        }

        // hCursor -> (Icon, Hotspot)
        static readonly Dictionary<IntPtr, Tuple<Bitmap, Point>> _cursors = new Dictionary<IntPtr, Tuple<Bitmap, Point>>();
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Offset">Offset from Origin of the Captured Area.</param>
        public static void Draw(Graphics g, Func<Point, Point> Transform = null)
        {
            // ReSharper disable once RedundantAssignment
            var cursorInfo = new CursorInfo { cbSize = Marshal.SizeOf<CursorInfo>() };

            if (!GetCursorInfo(out cursorInfo))
                return;

            if (cursorInfo.flags != CursorShowing)
                return;

            Bitmap icon;
            Point hotspot;

            if (_cursors.ContainsKey(cursorInfo.hCursor))
            {
                var tuple = _cursors[cursorInfo.hCursor];

                icon = tuple.Item1;
                hotspot = tuple.Item2;
            }
            else
            {
                var hIcon = CopyIcon(cursorInfo.hCursor);

                if (hIcon == IntPtr.Zero)
                    return;

                if (!GetIconInfo(hIcon, out var _icInfo))
                    return;

                icon = Icon.FromHandle(hIcon).ToBitmap();
                hotspot = new Point(_icInfo.xHotspot, _icInfo.yHotspot);

                _cursors.Add(cursorInfo.hCursor, Tuple.Create(icon, hotspot));

                DestroyIcon(hIcon);

                DeleteObject(_icInfo.hbmColor);
                DeleteObject(_icInfo.hbmMask);
            }

            var location = new Point(cursorInfo.ptScreenPos.X - hotspot.X,
                cursorInfo.ptScreenPos.Y - hotspot.Y);

            if (Transform != null)
                location = Transform(location);

            g.DrawImage(icon, new Rectangle(location, icon.Size));
        }
    }
}