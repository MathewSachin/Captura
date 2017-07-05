using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Screna.Native
{
    static class Gdi32
    {
        const string DllName = "gdi32.dll";

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
                
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);
                
        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(DllName)]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);
    }
}
