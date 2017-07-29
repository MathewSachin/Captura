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
        }

        protected readonly Func<Point, Point> _transform;
        readonly bool _includeCursor;

        Bitmap lastImage;

        /// <summary>
        /// Captures an Image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = new Bitmap(Width, Height);

            try
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    OnCapture(g);

                    if (_includeCursor)
                        MouseCursor.Draw(g, _transform);
                }

                lastImage = bmp;

                return bmp;
            }
            catch
            {
                if (lastImage != null)
                {
                    bmp.Dispose();

                    return lastImage;
                }

                return bmp;
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
        public virtual void Dispose() { }
    }
}
