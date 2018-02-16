using System;
using System.Drawing;

namespace Screna
{
    /// <summary>
    /// Captures the Region specified by a Rectangle.
    /// </summary>
    public class RegionProvider : IImageProvider
    {
        Rectangle _region;
        readonly ImagePool _imagePool;
        readonly bool _includeCursor;
        readonly Func<Point, Point> _transform;

        /// <summary>
        /// Creates a new instance of <see cref="RegionProvider"/>.
        /// </summary>
        public RegionProvider(Rectangle Region, bool IncludeCursor)
        {
            Region = Region.Even();

            _region = Region;
            _includeCursor = IncludeCursor;

            _transform = P => new Point(P.X - Region.X, P.Y - Region.Y);

            _imagePool = new ImagePool(Region.Width, Region.Height);
        }

        bool _outsideBounds;

        public void UpdateLocation(Point P)
        {
            _region.Location = P;

            _outsideBounds = !WindowProvider.DesktopRectangle.Contains(_region);
        }
        
        public IBitmapFrame Capture()
        {
            var bmp = _imagePool.Get();

            try
            {
                using (var editor = bmp.GetEditor())
                {
                    if (_outsideBounds)
                        editor.Graphics.Clear(Color.Transparent);

                    editor.Graphics.CopyFromScreen(_region.Location,
                        Point.Empty,
                        _region.Size,
                        CopyPixelOperation.SourceCopy);

                    if (_includeCursor)
                        MouseCursor.Draw(editor.Graphics, _transform);
                }
                
                return bmp;
            }
            catch
            {
                bmp.Dispose();

                return RepeatFrame.Instance;
            }
        }

        /// <summary>
        /// Height of Captured image.
        /// </summary>
        public int Height => _region.Height;

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        public int Width => _region.Width;

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            _imagePool.Dispose();
        }
    }
}
