using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Captures the Region specified by a Rectangle.
    /// </summary>
    public class RegionProvider : ImageProviderBase
    {
        readonly Rectangle _region;
        
        /// <summary>
        /// Creates a new instance of <see cref="RegionProvider"/>.
        /// </summary>
        /// <param name="Region">Region to Capture.</param>
        public RegionProvider(Rectangle Region, bool IncludeCursor)
            : base(Region.Size, P => new Point(P.X - Region.X, P.Y - Region.Y),  IncludeCursor)
        {
            _region = Region;
        }

        /// <summary>
        /// Capture an image.
        /// </summary>
        protected override void OnCapture(Graphics g)
        {
            g.CopyFromScreen(_region.Location,
                             Point.Empty,
                             _region.Size,
                             CopyPixelOperation.SourceCopy);
        }
    }
}
