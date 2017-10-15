using System;
using System.Collections.Generic;
using System.Linq;

namespace Screna
{
    public class ImagePool : IDisposable
    {
        readonly int _width, _height;

        public ImagePool(int Width, int Height)
        {
            _width = Width;
            _height = Height;
        }

        readonly List<ImageWrapper> _images = new List<ImageWrapper>();

        public ImageWrapper Get()
        {
            var any = _images.FirstOrDefault(M => M.Written);

            if (any != null)
            {
                any.Written = false;

                return any;
            }

            var img = new ImageWrapper(_width, _height);

            _images.Add(img);

            return img;
        }

        public void Dispose()
        {
            foreach (var image in _images)
            {
                image.Dispose();
            }
        }
    }
}