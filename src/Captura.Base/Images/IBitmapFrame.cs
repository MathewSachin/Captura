﻿using System;

namespace Captura
{
    public interface IBitmapFrame : IDisposable
    {
        int Width { get; }

        int Height { get; }

        void CopyTo(byte[] Buffer, int Length);

        void CopyTo(IntPtr Buffer, int Length);
    }
}