using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Draws over a Capured image.
    /// </summary>
    public interface IOverlay : IDisposable
    {
        /// <summary>
        /// Draws the Overlay.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object to draw on.</param>
        /// <param name="PointTransform">Point Transform Function.</param>
        void Draw(Graphics g, Func<Point, Point> PointTransform = null);
    }
}
