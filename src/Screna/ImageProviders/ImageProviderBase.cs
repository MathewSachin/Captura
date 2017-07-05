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
        /// <param name="Rectangle">A <see cref="Rectangle"/> representing the captured region.</param>
        protected ImageProviderBase(Rectangle Rectangle)
        {
            Width = Rectangle.Width;
            Height = Rectangle.Height;
        }

        /// <summary>
        /// Captures an Image.
        /// </summary>
        public Bitmap Capture()
        {
            var bmp = new Bitmap(Width, Height);

            using (var g = Graphics.FromImage(bmp))
                OnCapture(g);
            
            return bmp;
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
