using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable InconsistentNaming

namespace Captura.Native
{
    public static class User32
    {
        const string DllName = "user32.dll";
        
        [DllImport(DllName)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
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
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport(DllName)]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport(DllName)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport(DllName)]
        public static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        public static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, uint VirtualKey);

        [DllImport(DllName)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport(DllName)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(DllName)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

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
