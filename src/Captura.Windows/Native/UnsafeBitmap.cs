using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Captura.Native
{
    [StructLayout(LayoutKind.Sequential)]
    struct PixelData { public byte Blue, Green, Red, Alpha; }

    unsafe class UnsafeBitmap : IDisposable
    {
        readonly Bitmap _inputBitmap;
        BitmapData _bitmapData;
        byte* _pBase;
        readonly int _width;

        public UnsafeBitmap(Bitmap InputBitmap) 
        {
            _inputBitmap = InputBitmap;

            var bounds = new Rectangle(Point.Empty, _inputBitmap.Size);

            _width = bounds.Width * sizeof(PixelData);

            if (_width % 4 != 0)
                _width = 4 * (_width / 4 + 1);

            //Lock Image
            _bitmapData = _inputBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _pBase = (byte*)_bitmapData.Scan0;
        }

        public PixelData* this[int X, int Y] => (PixelData*)(_pBase + Y * _width + X * sizeof(PixelData));

        public void Dispose()
        {
            _inputBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}