using System;
using System.IO;
using Captura;

namespace Screna
{
    public class MultiDisposeFrame : IBitmapFrame
    {
        int _count;
        readonly IBitmapFrame _frame;
        readonly object _syncLock = new object();

        public MultiDisposeFrame(IBitmapFrame Frame, int Count)
        {
            if (_frame is RepeatFrame)
            {
                throw new NotSupportedException();
            }

            if (Count < 2)
            {
                throw new ArgumentException("Count should be atleast 2", nameof(Count));
            }

            _frame = Frame ?? throw new ArgumentNullException(nameof(Frame));
            _count = Count;
        }

        public void Dispose()
        {
            lock (_syncLock)
            {
                --_count;

                if (_count == 0)
                {
                    _frame.Dispose();
                }
            }
        }

        public void SaveGif(Stream Stream)
        {
            _frame.SaveGif(Stream);
        }

        public int Width => _frame.Width;
        public int Height => _frame.Height;

        public void CopyTo(byte[] Buffer, int Length)
        {
            _frame.CopyTo(Buffer, Length);
        }

        public IBitmapEditor GetEditor() => _frame.GetEditor();
    }
}