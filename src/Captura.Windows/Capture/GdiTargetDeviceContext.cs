﻿using System;
using System.Drawing;
using Captura;

namespace Screna
{
    class GdiTargetDeviceContext : ITargetDeviceContext
    {
        readonly IntPtr _hdcDest, _hBitmap;

        public GdiTargetDeviceContext(IntPtr SrcDc, int Width, int Height)
        {
            _hdcDest = Gdi32.CreateCompatibleDC(SrcDc);
            _hBitmap = Gdi32.CreateCompatibleBitmap(SrcDc, Width, Height);

            Gdi32.SelectObject(_hdcDest, _hBitmap);
        }

        public Type EditorType { get; } = typeof(GraphicsEditor);

        public void Dispose()
        {
            Gdi32.DeleteDC(_hdcDest);
            Gdi32.DeleteObject(_hBitmap);
        }

        public IntPtr GetDC() => _hdcDest;

        public IEditableFrame GetEditableFrame()
        {
            return new GraphicsEditor(Image.FromHbitmap(_hBitmap));
        }
    }
}
