using System;
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
        
        /// <summary>
        /// Draws this overlay.
        /// </summary>
        /// <param name="G">A <see cref="Graphics"/> object to draw upon.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public static void Draw(Graphics G, Func<Point, Point> Transform = null)
        {
            var iconPtr = GetIcon(Transform, out var location, out var disposer);

            if (iconPtr == IntPtr.Zero)
                return;

            var bmp = Icon.FromHandle(iconPtr).ToBitmap();

            try
            {
                G.DrawImage(bmp, new Rectangle(location, bmp.Size));
            }
            catch (ArgumentException) { }
            finally
            {
                disposer?.Invoke();
            }
        }

        public static void Draw(IntPtr DeviceContext, Func<Point, Point> Transform = null)
        {
            var iconPtr = GetIcon(Transform, out var location, out var disposer);

            if (iconPtr == IntPtr.Zero)
                return;

            try
            {
                User32.DrawIcon(DeviceContext, location.X, location.Y, iconPtr);
            }
            finally
            {
                disposer?.Invoke();
            }
        }

        static IntPtr GetIcon(Func<Point, Point> Transform, out Point Location, out Action Disposer)
        {
            Location = Point.Empty;
            Disposer = () => { };

            // ReSharper disable once RedundantAssignment
            // ReSharper disable once InlineOutVariableDeclaration
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

            Disposer = () => User32.DestroyIcon(hIcon);

            return hIcon;
        }
    }
}