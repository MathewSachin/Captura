using System;
using System.Runtime.InteropServices;

namespace Captura
{
    static class WindowBorderToggler
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr Window);

        [DllImport("gdi32.dll")]
        static extern bool PatBlt(IntPtr Window, int X, int Y, int Width, int Height, uint Operation);

        const int DstInvert = 0x0055_0009;

        public static void Toggle(IntPtr Window, int BorderThickness)
        {
            if (Window == IntPtr.Zero)
                return;

            var hdc = GetWindowDC(Window);

            var rect = new Screna.Window(Window).Rectangle;

            // Top
            PatBlt(hdc, 0, 0, rect.Width, BorderThickness, DstInvert);

            // Left
            PatBlt(hdc, 0, BorderThickness, BorderThickness, rect.Height - 2 * BorderThickness, DstInvert);

            // Right
            PatBlt(hdc, rect.Width - BorderThickness, BorderThickness, BorderThickness, rect.Height - 2 * BorderThickness, DstInvert);

            // Bottom
            PatBlt(hdc, 0, rect.Height - BorderThickness, rect.Width, BorderThickness, DstInvert);
        }
    }
}