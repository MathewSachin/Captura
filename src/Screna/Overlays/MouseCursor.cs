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
    public class MouseCursor : IOverlay
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

        MouseCursor() { }
        
        /// <summary>
        /// Singleton Instance of <see cref="MouseCursor"/>.
        /// </summary>
        public static MouseCursor Instance { get; } = new MouseCursor();

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
        Dictionary<IntPtr, Tuple<Bitmap, Point>> _cursors = new Dictionary<IntPtr, Tuple<Bitmap, Point>>();
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Offset">Offset from Origin of the Captured Area.</param>
        public void Draw(Graphics g, Point Offset = default(Point))
        {
            var _cursorInfo = new CursorInfo { cbSize = Marshal.SizeOf<CursorInfo>() };

            if (!GetCursorInfo(out _cursorInfo))
                return;

            if (_cursorInfo.flags != CursorShowing)
                return;

            Bitmap icon = null;
            Point hotspot = Point.Empty;

            if (_cursors.ContainsKey(_cursorInfo.hCursor))
            {
                var tuple = _cursors[_cursorInfo.hCursor];

                icon = tuple.Item1;
                hotspot = tuple.Item2;
            }
            else
            {
                var _hIcon = CopyIcon(_cursorInfo.hCursor);

                if (_hIcon == IntPtr.Zero)
                    return;

                if (!GetIconInfo(_hIcon, out var _icInfo))
                    return;

                icon = Icon.FromHandle(_hIcon).ToBitmap();
                hotspot = new Point(_icInfo.xHotspot, _icInfo.yHotspot);

                _cursors.Add(_cursorInfo.hCursor, Tuple.Create(icon, hotspot));

                DestroyIcon(_hIcon);

                DeleteObject(_icInfo.hbmColor);
                DeleteObject(_icInfo.hbmMask);
            }

            var location = new Point(_cursorInfo.ptScreenPos.X - Offset.X - hotspot.X,
                _cursorInfo.ptScreenPos.Y - Offset.Y - hotspot.Y);

            g.DrawImage(icon, new Rectangle(location, icon.Size));
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose() { }
    }
}