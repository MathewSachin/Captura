using Screna.Native;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Screna
{
    /// <summary>
    /// Contains methods for taking ScreenShots
    /// </summary>
    public static class ScreenShot
    {
        #region PInvoke
        const string DllName = "user32.dll";

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(IntPtr hWnd, int dWAttribute, ref RECT pvAttribute, int cbAttribute);
        
        [DllImport(DllName)]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport(DllName)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport(DllName)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(DllName)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport(DllName)]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        /// <summary>
        /// Captures a Specific <see cref="Screen"/>.
        /// </summary>
        /// <param name="Screen">The <see cref="Screen"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <param name="Managed">Whether to use Managed or Unmanaged Procedure.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(Screen Screen, bool IncludeCursor = false, bool Managed = true)
        {
            return Capture(Screen.Bounds, IncludeCursor, Managed);
        }
        
        /// <summary>
        /// Captures a Specific Window.
        /// </summary>
        /// <param name="Window">The <see cref="Window"/> to Capture</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(Window Window, bool IncludeCursor = false)
        {
            User32.GetWindowRect(Window.Handle, out var r);
            var region = r.ToRectangle();

            IntPtr hSrc = GetWindowDC(Window.Handle),
                hDest = Gdi32.CreateCompatibleDC(hSrc),
                hBmp = Gdi32.CreateCompatibleBitmap(hSrc, region.Width, region.Height),
                hOldBmp = Gdi32.SelectObject(hDest, hBmp);

            Gdi32.BitBlt(hDest, 0, 0,
                region.Width, region.Height,
                hSrc,
                region.Left, region.Top,
                CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

            var bmp = Image.FromHbitmap(hBmp);

            Gdi32.SelectObject(hDest, hOldBmp);
            Gdi32.DeleteObject(hBmp);
            Gdi32.DeleteDC(hDest);
            Gdi32.DeleteDC(hSrc);

            var clone = bmp.Clone(new Rectangle(Point.Empty, bmp.Size), PixelFormat.Format24bppRgb);

            if (IncludeCursor)
                MouseCursor.Instance.Draw(clone, region.Location);

            return clone;
        }

        /// <summary>
        /// Captures a Specific <see cref="Form"/>.
        /// </summary>
        /// <param name="Form">The <see cref="Form"/> to Capture</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(Form Form, bool IncludeCursor = false) => Capture(new Window(Form.Handle), IncludeCursor);

        /// <summary>
        /// Captures the entire Desktop.
        /// </summary>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <param name="Managed">Whether to use Managed or Unmanaged Procedure.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(bool IncludeCursor = false, bool Managed = true)
        {
            return Capture(WindowProvider.DesktopRectangle, IncludeCursor, Managed);
        }

        /// <summary>
        /// Capture transparent Screenshot of a Window.
        /// </summary>
        /// <param name="Window">The <see cref="Window"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include Mouse Cursor.</param>
        public static unsafe Bitmap CaptureTransparent(Window Window, bool IncludeCursor = false)
        {
            var tmpColour = Color.White;

            var backdrop = new Form
            {
                AllowTransparency = true,
                BackColor = tmpColour,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Opacity = 0
            };

            var r = new RECT();

            const int extendedFrameBounds = 0;

            if (DwmGetWindowAttribute(Window.Handle, extendedFrameBounds, ref r, sizeof(RECT)) != 0)
                // DwmGetWindowAttribute() failed, usually means Aero is disabled so we fall back to GetWindowRect()
                User32.GetWindowRect(Window.Handle, out r);

            var R = r.ToRectangle();

            // Add a 100px margin for window shadows. Excess transparency is trimmed out later
            R.Inflate(100, 100);

            // This check handles if the window is outside of the visible screen
            R.Intersect(WindowProvider.DesktopRectangle);

            ShowWindow(backdrop.Handle, 4);
            SetWindowPos(backdrop.Handle, Window.Handle,
                R.Left, R.Top,
                R.Width, R.Height,
                SetWindowPositionFlags.NoActivate);
            backdrop.Opacity = 1;
            Application.DoEvents();

            // Capture screenshot with white background
            using (var whiteShot = CaptureRegionUnmanaged(R))
            {
                backdrop.BackColor = Color.Black;
                Application.DoEvents();

                // Capture screenshot with black background
                using (var blackShot = CaptureRegionUnmanaged(R))
                {
                    backdrop.Dispose();

                    var transparentImage = Extensions.DifferentiateAlpha(whiteShot, blackShot);
                    if (IncludeCursor)
                        MouseCursor.Instance.Draw(transparentImage, R.Location);
                    return transparentImage.CropEmptyEdges();
                }
            }
        }

        /// <summary>
        /// Capture transparent Screenshot of a Window.
        /// </summary>
        /// <param name="Window">The <see cref="Window"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include Mouse Cursor.</param>
        /// <param name="DoResize">
        /// Whether to Capture at another size.
        /// The Window is sized to the specified Resize Dimensions, Captured and resized back to original size.
        /// </param>
        /// <param name="ResizeWidth">Capture Width.</param>
        /// <param name="ResizeHeight">Capture Height.</param>
        public static Bitmap CaptureTransparent(Window Window, bool IncludeCursor, bool DoResize, int ResizeWidth, int ResizeHeight)
        {
            var startButtonHandle = User32.FindWindow("Button", "Start");
            var taskbarHandle = User32.FindWindow("Shell_TrayWnd", null);

            var canResize = DoResize && User32.GetWindowLong(Window.Handle, GetWindowLongValue.Style).HasFlag(WindowStyles.SizeBox);

            try
            {
                // Hide the taskbar, just incase it gets in the way
                if (Window.Handle != startButtonHandle && Window.Handle != taskbarHandle)
                {
                    ShowWindow(startButtonHandle, 0);
                    ShowWindow(taskbarHandle, 0);
                    Application.DoEvents();
                }

                if (IsIconic(Window.Handle))
                {
                    ShowWindow(Window.Handle, 1);
                    Thread.Sleep(300); // Wait for window to be restored
                }
                else
                {
                    ShowWindow(Window.Handle, 5);
                    Thread.Sleep(100);
                }

                SetForegroundWindow(Window.Handle);

                var r = new RECT();

                if (canResize)
                {
                    User32.GetWindowRect(Window.Handle, out r);

                    SetWindowPos(Window.Handle, IntPtr.Zero, r.Left, r.Top, ResizeWidth, ResizeHeight, SetWindowPositionFlags.ShowWindow);

                    Thread.Sleep(100);
                }

                var s = CaptureTransparent(Window, IncludeCursor);

                var R = r.ToRectangle();

                if (canResize)
                    SetWindowPos(Window.Handle, IntPtr.Zero,
                        R.Left, R.Top,
                        R.Width, R.Height,
                        SetWindowPositionFlags.ShowWindow);

                return s;
            }
            finally
            {
                if (Window.Handle != startButtonHandle && Window.Handle != taskbarHandle)
                {
                    ShowWindow(startButtonHandle, 1);
                    ShowWindow(taskbarHandle, 1);
                }
            }
        }

        #region Region
        static Bitmap CaptureRegionUnmanaged(Rectangle Region, bool IncludeCursor = false)
        {
            IntPtr hSrc = Gdi32.CreateDC("DISPLAY", null, null, 0),
                hDest = Gdi32.CreateCompatibleDC(hSrc),
                hBmp = Gdi32.CreateCompatibleBitmap(hSrc, Region.Width, Region.Height),
                hOldBmp = Gdi32.SelectObject(hDest, hBmp);

            Gdi32.BitBlt(hDest, 0, 0,
                Region.Width, Region.Height,
                hSrc,
                Region.Left, Region.Top,
                CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

            var bmp = Image.FromHbitmap(hBmp);

            Gdi32.SelectObject(hDest, hOldBmp);
            Gdi32.DeleteObject(hBmp);
            Gdi32.DeleteDC(hDest);
            Gdi32.DeleteDC(hSrc);

            var clone = bmp.Clone(new Rectangle(Point.Empty, bmp.Size), PixelFormat.Format24bppRgb);

            if (IncludeCursor)
                MouseCursor.Instance.Draw(clone, Region.Location);

            return clone;
        }

        static Bitmap CaptureRegionManaged(Rectangle Region, bool IncludeCursor = false)
        {
            var bmp = new Bitmap(Region.Width, Region.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Region.Location, Point.Empty, Region.Size, CopyPixelOperation.SourceCopy);

                if (IncludeCursor)
                    MouseCursor.Instance.Draw(g, Region.Location);

                g.Flush();
            }

            return bmp;
        }

        /// <summary>
        /// Captures a Specific Region.
        /// </summary>
        /// <param name="Region">A <see cref="Rectangle"/> specifying the Region to Capture.</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <param name="Managed">Whether to use Managed or Unmanaged Procedure.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(Rectangle Region, bool IncludeCursor = false, bool Managed = true)
        {
            return Managed ? CaptureRegionManaged(Region, IncludeCursor) : CaptureRegionUnmanaged(Region, IncludeCursor);
        }
        #endregion
    }
}
