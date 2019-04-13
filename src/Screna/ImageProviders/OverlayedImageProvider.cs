using System;
using System.Drawing;
using Captura;

namespace Screna
{
    /// <summary>
    /// Applies Overlays on an <see cref="IImageProvider"/>.
    /// </summary>
    public class OverlayedImageProvider : IImageProvider
    {
        IOverlay[] _overlays;
        public IImageProvider ImageProvider { get; private set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="OverlayedImageProvider"/>.
        /// </summary>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> to apply the Overlays on.</param>
        /// <param name="Overlays">Array of <see cref="IOverlay"/>(s) to apply.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public OverlayedImageProvider(IImageProvider ImageProvider, params IOverlay[] Overlays)
        {
            this.ImageProvider = ImageProvider ?? throw new ArgumentNullException(nameof(ImageProvider));
            _overlays = Overlays ?? throw new ArgumentNullException(nameof(Overlays));

            Width = ImageProvider.Width;
            Height = ImageProvider.Height;
        }

        /// <inheritdoc />
        public IEditableFrame Capture()
        {
            var bmp = ImageProvider.Capture();
            
            // Overlays should have already been drawn on previous frame
            if (bmp is RepeatFrame)
            {
                return bmp;
            }

            if (_overlays != null)
            {
                foreach (var overlay in _overlays)
                    overlay?.Draw(bmp, ImageProvider.PointTransform);
            }
            
            return bmp;
        }
        
        /// <inheritdoc />
        public int Height { get; }

        /// <inheritdoc />
        public int Width { get; }

        public Func<Point, Point> PointTransform { get; } = P => P;

        /// <inheritdoc />
        public void Dispose()
        {
            ImageProvider.Dispose();

            foreach (var overlay in _overlays)
                overlay?.Dispose();

            ImageProvider = null;
            _overlays = null;
        }
    }
}
