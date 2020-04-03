using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Captura.Native;

namespace Captura.Video
{
    /// <summary>
    /// Draws the MouseCursor on an Image
    /// </summary>
    static class MouseCursor
    {
        const int CursorShowing = 1;
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="G">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public static void Draw(Graphics G, Func<Point, Point> Transform = null)
        {
            var hIcon = GetIcon(Transform, out var location);

            if (hIcon == IntPtr.Zero)
                return;

            var bmp = Icon.FromHandle(hIcon).ToBitmap();
            User32.DestroyIcon(hIcon);

            try
            {
                using (bmp)
                    G.DrawImage(bmp, new Rectangle(location, bmp.Size));
            }
            catch (ArgumentException) { }
        }

        public static void Draw(IntPtr DeviceContext, Func<Point, Point> Transform = null)
        {
            var hIcon = GetIcon(Transform, out var location);

            if (hIcon == IntPtr.Zero)
                return;

            try
            {
                User32.DrawIconEx(DeviceContext,
                    location.X, location.Y,
                    hIcon,
                    0, 0, 0, IntPtr.Zero,
                    DrawIconExFlags.Normal);
            }
            finally
            {
                User32.DestroyIcon(hIcon);
            }
        }

        static IntPtr GetIcon(Func<Point, Point> Transform, out Point Location)
        {
            Location = Point.Empty;

            var cursorInfo = new CursorInfo {cbSize = Marshal.SizeOf<CursorInfo>()};

            if (!User32.GetCursorInfo(ref cursorInfo))
                return IntPtr.Zero;

            if (cursorInfo.flags != CursorShowing)
                return IntPtr.Zero;

            var hIcon = User32.CopyIcon(cursorInfo.hCursor);

            if (hIcon == IntPtr.Zero)
                return IntPtr.Zero;

            if (!User32.GetIconInfo(hIcon, out var icInfo))
                return IntPtr.Zero;

            var hotspot = new Point(icInfo.xHotspot, icInfo.yHotspot);

            Location = new Point(cursorInfo.ptScreenPos.X - hotspot.X,
                cursorInfo.ptScreenPos.Y - hotspot.Y);

            if (Transform != null)
                Location = Transform(Location);

            Gdi32.DeleteObject(icInfo.hbmColor);
            Gdi32.DeleteObject(icInfo.hbmMask);

            return hIcon;
        }
    }
}