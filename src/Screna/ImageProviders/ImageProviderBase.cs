using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// An abstract implementation of <see cref="IImageProvider"/> interface.
    /// </summary>
    public abstract class ImageProviderBase : IImageProvider
    {
        /// <summary>
        /// Constructor for <see cref="ImageProviderBase"/>.
        /// </summary>
        /// <param name="Size">Size of the captured region.</param>
        protected ImageProviderBase(Size Size, Func<Point, Point> Transform, bool IncludeCursor)
        {
            Width = Size.Width;
            Height = Size.Height;

            _transform = Transform;
            _includeCursor = IncludeCursor;
            _imagePool = new ImagePool(Width, Height);
        }

        protected readonly Func<Point, Point> _transform;
        readonly bool _includeCursor;

        readonly ImagePool _imagePool;

        /// <summary>
        /// Captures an Image.
        /// </summary>
        public ImageWrapper Capture()
        {
            var img = _imagePool.Get();

            try
            {
                var g = img.Graphics;

                OnCapture(g);

                if (_includeCursor)
                    MouseCursor.Draw(g, _transform);

                g.Flush();

                return img;
            }
            catch
            {
                return ImageWrapper.Repeat;
            }
        }

        /// <summary>
        /// Implemented by derived classes for the actual capture process.
        /// </summary>
        protected abstract void OnCapture(Graphics g);

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
            _imagePool.Dispose();
        }
    }
}
