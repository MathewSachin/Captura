﻿using System;
using Captura;
using Captura.Models;

namespace Screna
{
    public class MultiDisposeFrame : IBitmapFrame, IFrameWrapper
    {
        int _count;

        public IBitmapFrame Frame { get; }

        readonly object _syncLock = new object();

        public MultiDisposeFrame(IBitmapFrame Frame, int Count)
        {
            if (this.Frame is RepeatFrame)
            {
                throw new NotSupportedException();
            }

            if (Count < 2)
            {
                throw new ArgumentException("Count should be atleast 2", nameof(Count));
            }

            this.Frame = Frame ?? throw new ArgumentNullException(nameof(Frame));
            _count = Count;
        }

        public void Dispose()
        {
            lock (_syncLock)
            {
                --_count;

                if (_count == 0)
                {
                    Frame.Dispose();
                }
            }
        }

        public int Width => Frame.Width;
        public int Height => Frame.Height;

        public void CopyTo(byte[] Buffer, int Length)
        {
            Frame.CopyTo(Buffer, Length);
        }

        public void CopyTo(IntPtr Buffer, int Length)
        {
            Frame.CopyTo(Buffer, Length);
        }
    }
}