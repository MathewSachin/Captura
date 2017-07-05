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
        readonly Func<Point> _offset;

        /// <summary>
        /// Creates a new instance of <see cref="OverlayedImageProvider"/>.
        /// </summary>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> to apply the Overlays on.</param>
        /// <param name="Overlays">Array of <see cref="IOverlay"/>(s) to apply.</param>
        /// <param name="Offset">The Offset of the Captured region.</param>
        public OverlayedImageProvider(IImageProvider ImageProvider, Point Offset, params IOverlay[] Overlays)
            : this(ImageProvider, () => Offset, Overlays) { }

        /// <summary>
        /// Creates a new instance of <see cref="OverlayedImageProvider"/>.
        /// </summary>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> to apply the Overlays on.</param>
        /// <param name="Overlays">Array of <see cref="IOverlay"/>(s) to apply.</param>
        /// <param name="Offset">Function returning Offset of the Captured region.</param>
        public OverlayedImageProvider(IImageProvider ImageProvider, Func<Point> Offset, params IOverlay[] Overlays)
        {
            _imageProvider = ImageProvider;
            _overlays = Overlays;
            _offset = Offset;

            Width = ImageProvider.Width;
            Height = ImageProvider.Height;
        }

        /// <summary>
        /// Captures an Image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = _imageProvider.Capture();

            using (var g = Graphics.FromImage(bmp))
            {
                if (_overlays != null)
                    foreach (var overlay in _overlays)
                        overlay?.Draw(g, _offset());
            }

            return bmp;
        }
        
        /// <summary>
        /// Height of Captured image.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public virtual void Dispose()
        {
            _imageProvider.Dispose();

            if (_overlays == null)
                return;

            foreach (var overlay in _overlays)
                overlay?.Dispose();
        }
    }
}
