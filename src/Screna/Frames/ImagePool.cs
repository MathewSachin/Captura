using System;
using System.Collections.Generic;
using System.Drawing;

namespace Screna
{
    public class ImagePool : IDisposable
    {
        public ImagePool(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public int Width { get; }

        public int Height { get; }

        readonly List<ReusableFrame> _frames = new List<ReusableFrame>();
        readonly Queue<IBitmapFrame> _pool = new Queue<IBitmapFrame>();

        public IBitmapFrame Get()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                    return _pool.Dequeue();

                var frame = new ReusableFrame(new Bitmap(Width, Height));

                frame.Released += () =>
                {
                    lock (_pool)
                    {
                        _pool.Enqueue(frame);
                    }
                };

                _frames.Add(frame);

                return frame;
            }
        }

        public void Dispose()
        {
            foreach (var frame in _frames)
            {
                frame.Destroy();
            }
        }
    }
}