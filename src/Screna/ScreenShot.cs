using System.Drawing;

namespace Captura.Video
{
    /// <summary>
    /// Contains methods for taking ScreenShots
    /// </summary>
    public static class ScreenShot
    {
        /// <summary>
        /// Captures the entire Desktop.
        /// </summary>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static IBitmapImage Capture(bool IncludeCursor = false)
        {
            var platformServices = ServiceProvider.Get<IPlatformServices>();

            return Capture(platformServices.DesktopRectangle, IncludeCursor);
        }

        /// <summary>
        /// Capture transparent Screenshot of a Window.
        /// </summary>
        /// <param name="Window">The <see cref="IWindow"/> to Capture.</param>
        /// <param name="IncludeCursor">Whether to include Mouse Cursor.</param>
        public static IBitmapImage CaptureTransparent(IWindow Window, bool IncludeCursor = false)
        {
            var platformServices = ServiceProvider.Get<IPlatformServices>();

            return platformServices.CaptureTransparent(Window, IncludeCursor);
        }
        
        /// <summary>
        /// Captures a Specific Region.
        /// </summary>
        /// <param name="Region">A <see cref="Rectangle"/> specifying the Region to Capture.</param>
        /// <param name="IncludeCursor">Whether to include the Mouse Cursor.</param>
        /// <returns>The Captured Image.</returns>
        public static IBitmapImage Capture(Rectangle Region, bool IncludeCursor = false)
        {
            var platformServices = ServiceProvider.Get<IPlatformServices>();

            return platformServices.Capture(Region, IncludeCursor);
        }
    }
}
