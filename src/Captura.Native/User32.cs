using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Captura.Native
{
    public static class User32
    {
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        public static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, uint VirtualKey);

        [DllImport(DllName)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport(DllName)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(DllName)]
        public static extern bool SetProcessDPIAware();
    }
}
