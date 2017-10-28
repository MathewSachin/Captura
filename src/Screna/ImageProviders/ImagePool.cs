using System;
using System.Collections.Generic;

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

        readonly Queue<ImageWrapper> _imageQueue = new Queue<ImageWrapper>();
        readonly List<ImageWrapper> _images = new List<ImageWrapper>();
        
        public ImageWrapper Get()
        {
            lock (_imageQueue)
            {
                if (_imageQueue.Count == 0)
                {
                    var img = new ImageWrapper(_width, _height);

                    _images.Add(img);

                    img.Freed += () =>
                    {
                        lock (_imageQueue)
                            _imageQueue.Enqueue(img);
                    };

                    return img;
                }

                return _imageQueue.Dequeue();
            }
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