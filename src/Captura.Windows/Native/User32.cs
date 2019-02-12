using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Captura.Native;

// ReSharper disable InconsistentNaming

namespace Captura
{
    static class User32
    {
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport(DllName)]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, GetWindowLongValue nIndex);

        [DllImport(DllName)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport(DllName)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(DllName)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(DllName)]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport(DllName)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport(DllName)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowEnum uCmd);

        [DllImport(DllName)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsZoomed(IntPtr hWnd);
    }
}