using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Captura.Native
{
    static class Gdi32
    {
        const string DllName = "gdi32.dll";

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(DllName)]
        public static extern bool BitBlt(IntPtr hObject, int XDest, int YDest, int Width, int Height, IntPtr ObjectSource, int XSrc, int YSrc, int Op);

        [DllImport(DllName)]
        public static extern bool StretchBlt(IntPtr hObject, int XDest, int YDest, int WDest, int HDest, IntPtr ObjectSource, int XSrc, int YSrc, int WSrc, int HSrc, int Op);

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Height);

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
}