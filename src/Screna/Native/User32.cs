using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Screna.Native
{
    static class User32
    {
        const string DllName = "user32.dll";
        
        [DllImport(DllName)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport(DllName)]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, GetWindowLongValue nIndex);
        
        [DllImport(DllName)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    }
}
