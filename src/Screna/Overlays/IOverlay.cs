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
        /// <param name="Offset">The Offset of the captured region.</param>
        void Draw(Graphics g, Point Offset = default(Point));
    }
}
