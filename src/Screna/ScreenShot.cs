using System;
using System.Drawing;
using System.Windows.Forms;
using Captura.Models;

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

            var backdrop = new WindowScreenShotBackdrop(Window);

            backdrop.ShowWhite();

            var r = backdrop.Rectangle;

            // Capture screenshot with white background
            using (var whiteShot = Capture(r))
            {
                backdrop.ShowBlack();

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
