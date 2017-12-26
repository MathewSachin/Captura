using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Applies Overlays on an <see cref="IImageProvider"/>.
    /// </summary>
    public class OverlayedImageProvider : IImageProvider
    {
        readonly IOverlay[] _overlays;
        readonly IImageProvider _imageProvider;
        readonly Func<Point, Point> _transform;
        
        /// <summary>
        /// Creates a new instance of <see cref="OverlayedImageProvider"/>.
        /// </summary>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> to apply the Overlays on.</param>
        /// <param name="Overlays">Array of <see cref="IOverlay"/>(s) to apply.</param>
        /// <param name="Transform">Point Transform Function.</param>
        public OverlayedImageProvider(IImageProvider ImageProvider, Func<Point, Point> Transform, params IOverlay[] Overlays)
        {
            _imageProvider = ImageProvider;
            _overlays = Overlays;
            _transform = Transform;

            Width = ImageProvider.Width;
            Height = ImageProvider.Height;
        }

        int _lastFrameHash;

        /// <summary>
        /// Captures an Image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = _imageProvider.Capture();
            
            var hash = bmp.GetHashCode();

            if (_lastFrameHash == hash)
            {
                return bmp;
            }

            _lastFrameHash = hash;

            using (var g = Graphics.FromImage(bmp))
            {
                if (_overlays != null)
                    foreach (var overlay in _overlays)
                        overlay?.Draw(g, _transform);
            }

            return bmp;
        }
        
        /// <inheritdoc />
        public int Height { get; }

        /// <inheritdoc />
        public int Width { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _imageProvider.Dispose();

            if (_overlays == null)
                return;

            foreach (var overlay in _overlays)
                overlay?.Dispose();
        }
    }
}
