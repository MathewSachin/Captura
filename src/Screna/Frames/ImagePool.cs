using System;
using System.Collections.Generic;
using System.Drawing;
using Captura;

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

        const int MaxFreeFrames = 5;

        public IBitmapFrame Get()
        {
            lock (_pool)
            {
                DestroyFreeFrames();

                if (_pool.Count > 0)
                    return _pool.Dequeue();

                var frame = new ReusableFrame(new Bitmap(Width, Height));

                frame.Released += () =>
                {
                    lock (_pool)
                    {
                        _pool.Enqueue(frame);

                        DestroyFreeFrames();
                    }
                };

                _frames.Add(frame);

                Console.WriteLine($"New Frame Allocated: {_frames.Count - 1}");

                return frame;
            }
        }

        void DestroyFreeFrames()
        {
            while (_pool.Count > MaxFreeFrames)
            {
                if (_pool.Dequeue() is ReusableFrame freeFrame)
                {
                    _frames.Remove(freeFrame);

                    freeFrame.Destroy();

                    Console.WriteLine($"Frame Destroyed: {_frames.Count - 1}");
                }
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