using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AeroShot
{
    [StructLayout(LayoutKind.Sequential)]
    struct PixelData
    {
        public byte Blue, Green, Red, Alpha;

        public void SetAll(byte b)
        {
            Red = b;
            Green = b;
            Blue = b;
            Alpha = 255;
        }
    }

    unsafe class UnsafeBitmap
    {
        readonly Bitmap _inputBitmap;
        BitmapData _bitmapData;
        byte* _pBase = null;
        int _width;

        public UnsafeBitmap(Bitmap inputBitmap) { _inputBitmap = inputBitmap; }

        public void LockImage()
        {
            var bounds = new Rectangle(Point.Empty, _inputBitmap.Size);

            _width = bounds.Width * sizeof(PixelData);
            if (_width % 4 != 0) _width = 4 * (_width / 4 + 1);

            //Lock Image
            _bitmapData = _inputBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _pBase = (byte*)_bitmapData.Scan0.ToPointer();
        }

        public PixelData* GetPixel(int x, int y) { return (PixelData*)(_pBase + y * _width + x * sizeof(PixelData)); }

        public void UnlockImage()
        {
            _inputBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}