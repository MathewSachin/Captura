using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screna
{
    /// <summary>
    /// Capture a Specific Screen.
    /// </summary>
    public class ScreenProvider : ImageProviderBase
    {
        readonly Screen _screen;

        /// <summary>
        /// Creates a new instance of <see cref="ScreenProvider"/>.
        /// </summary>
        /// <param name="Screen">The Screen to Capture.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Screen"/> is null.</exception>
        public ScreenProvider(Screen Screen, bool IncludeCursor)
            : base(Screen.Bounds.Size, P => new Point(P.X - Screen.Bounds.X, P.Y - Screen.Bounds.Y), IncludeCursor)
        {
            _screen = Screen;
        }

        /// <summary>
        /// Capture Frame.
        /// </summary>
        protected override void OnCapture(Graphics g)
        {
            using (var img = ScreenShot.Capture(_screen))
                g.DrawImage(img, Point.Empty);
        }
    }
}
