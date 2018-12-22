using System;
using System.Drawing;

namespace Captura
{
    /// <summary>
    /// Draws over a Capured image.
    /// </summary>
    public interface IOverlay : IDisposable
    {
        /// <summary>
        /// Draws the Overlay.
        /// </summary>
        void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null);
    }
}
