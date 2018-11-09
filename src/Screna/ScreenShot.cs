using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Captura.Models;
using Captura.Native;

namespace Screna
{
    /// <summary>
    /// Contains methods for taking ScreenShots
    /// </summary>
    public static class ScreenShot
    {
        /// <summary>
        /// Captures a Specific <see cref="Screen"/>.
        /// </summary>
        /// <param name="Screen">The <see cref="IScreen"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(IScreen Screen, bool IncludeCursor = false)
        {
            if (Screen == null)
                throw new ArgumentNullException(nameof(Screen));

            return Capture(Screen.Rectangle, IncludeCursor);
        }

        public static Bitmap Capture(IWindow Window, bool IncludeCursor = false)
        {
            if (Window == null)
                throw new ArgumentNullException(nameof(Window));

            return Capture(Window.Rectangle, IncludeCursor);
        }

        /// <summary>
        /// Captures the entire Desktop.
        /// </summary>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(bool IncludeCursor = false)
        {
            return Capture(WindowProvider.DesktopRectangle, IncludeCursor);
        }

        /// <summary>
        /// Capture transparent Screenshot of a Window.
        /// </summary>
        /// <param name="Window">The <see cref="IWindow"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include Mouse Cursor.</param>
        public static Bitmap CaptureTransparent(IWindow Window, bool IncludeCursor = false)
        {
            if (Window == null)
                throw new ArgumentNullException(nameof(Window));

            var backdrop = new Form
            {
                AllowTransparency = true,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Opacity = 0
            };

            var r = Window.Rectangle;

            // Add a 100px margin for window shadows. Excess transparency is trimmed out later
            r.Inflate(100, 100);

            // This check handles if the window is outside of the visible screen
            r.Intersect(WindowProvider.DesktopRectangle);

            User32.ShowWindow(backdrop.Handle, 4);
            User32.SetWindowPos(backdrop.Handle, Window.Handle,
                r.Left, r.Top,
                r.Width, r.Height,
                SetWindowPositionFlags.NoActivate);
            backdrop.Opacity = 1;
            Application.DoEvents();

            // Capture screenshot with white background
            using (var whiteShot = Capture(r))
            {
                backdrop.BackColor = Color.Black;
                Application.DoEvents();

                // Capture screenshot with black background
                using (var blackShot = Capture(r))
                {
                    backdrop.Dispose();

                    var transparentImage = Extensions.DifferentiateAlpha(whiteShot, blackShot);

                    if (transparentImage == null)
                        return null;

                    // Include Cursor only if within window
                    if (IncludeCursor && r.Contains(MouseCursor.CursorPosition))
                    {
                        using (var g = Graphics.FromImage(transparentImage))
                            MouseCursor.Draw(g, P => new Point(P.X - r.X, P.Y - r.Y));
                    }

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
        public static Bitmap CaptureTransparent(IWindow Window, bool IncludeCursor, bool DoResize, int ResizeWidth, int ResizeHeight)
        {
            if (Window == null)
                throw new ArgumentNullException(nameof(Window));

            var startButtonHandle = User32.FindWindow("Button", "Start");
            var taskbarHandle = User32.FindWindow("Shell_TrayWnd", null);

            var canResize = DoResize && User32.GetWindowLong(Window.Handle, GetWindowLongValue.Style).HasFlag(WindowStyles.SizeBox);

            try
            {
                // Hide the taskbar, just incase it gets in the way
                if (Window.Handle != startButtonHandle && Window.Handle != taskbarHandle)
                {
                    User32.ShowWindow(startButtonHandle, 0);
                    User32.ShowWindow(taskbarHandle, 0);
                    Application.DoEvents();
                }

                if (User32.IsIconic(Window.Handle))
                {
                    User32.ShowWindow(Window.Handle, 1);
                    Thread.Sleep(300); // Wait for window to be restored
                }
                else
                {
                    User32.ShowWindow(Window.Handle, 5);
                    Thread.Sleep(100);
                }

                User32.SetForegroundWindow(Window.Handle);

                var r = Window.Rectangle;

                void SetSize(int Width, int Height)
                {
                    User32.SetWindowPos(Window.Handle,
                        IntPtr.Zero,
                        r.Left, r.Top,
                        Width, Height,
                        SetWindowPositionFlags.ShowWindow);
                }

                if (canResize)
                {
                    SetSize(ResizeWidth, ResizeHeight);

                    Thread.Sleep(100);
                }

                var s = CaptureTransparent(Window, IncludeCursor);

                if (canResize)
                {
                    SetSize(r.Width, r.Height);
                }

                return s;
            }
            finally
            {
                if (Window.Handle != startButtonHandle && Window.Handle != taskbarHandle)
                {
                    User32.ShowWindow(startButtonHandle, 1);
                    User32.ShowWindow(taskbarHandle, 1);
                }
            }
        }

        /// <summary>
        /// Captures a Specific Region.
        /// </summary>
        /// <param name="Region">A <see cref="Rectangle"/> specifying the Region to Capture.</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static Bitmap Capture(Rectangle Region, bool IncludeCursor = false)
        {
            var bmp = new Bitmap(Region.Width, Region.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Region.Location, Point.Empty, Region.Size, CopyPixelOperation.SourceCopy);

                if (IncludeCursor)
                    MouseCursor.Draw(g, P => new Point(P.X - Region.X, P.Y - Region.Y));

                g.Flush();
            }

            return bmp;
        }
    }
}
