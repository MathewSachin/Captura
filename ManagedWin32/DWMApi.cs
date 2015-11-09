using System;
using System.Runtime.InteropServices;

namespace Captura
{
    public enum DwmWindowAttribute { ExtendedFrameBounds }

    public static class DWMApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dWAttribute, ref RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        static extern bool DwmIsCompositionEnabled();
    }
}
