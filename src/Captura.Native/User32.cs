using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Captura.Native
{
    public static class User32
    {
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport(DllName)]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport(DllName)]
        public static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        public static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, uint VirtualKey);

        [DllImport(DllName)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport(DllName)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(DllName)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool FillRect(IntPtr hDC, ref RECT Rect, IntPtr Brush);

        [DllImport(DllName)]
        public static extern bool SetProcessDPIAware();
    }
}
