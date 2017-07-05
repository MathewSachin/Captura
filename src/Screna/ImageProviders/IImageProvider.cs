using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Screna
{
    /// <summary>
    /// Provides images.
    /// Must provide in <see cref="PixelFormat.Format32bppRgb"/>
    /// </summary>
    public interface IImageProvider : IDisposable
    {
        /// <summary>
        /// Capture an image.
        /// </summary>
        Bitmap Capture();

        /// <summary>
        /// Height of Captured image.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        int Width { get; }
    }
}