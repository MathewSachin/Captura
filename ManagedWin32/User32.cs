using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Captura
{
    #region Enumerations
    public enum GetWindowEnum { Owner = 4 }

    public enum SetWindowPositionFlags
    {
        NoMove = 0x2,
        NoSize = 1,
        NoZOrder = 0x4,
        ShowWindow = 0x400,
        NoActivate = 0x0010
    }

    public enum WindowStyles : uint
    {
        WS_CHILD = 0x40000000,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_APPWINDOW = 0x00040000,
    }

    public enum GetWindowLongValue
    {
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20,
    }
    #endregion

    #region Structures
    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public int cbSize;
        public int flags;
        public IntPtr hCursor;
        public Point ptScreenPos;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int Left, int Top, int Right, int Bottom)
        {
            this = new RECT()
            {
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom
            };
        }
    }
    #endregion

    #region Delegates
    public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    #endregion

    static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        public const int CURSOR_SHOWING = 0x00000001;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32", SetLastError = true)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32", SetLastError = true)]
        public extern static uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, GetWindowLongValue nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowEnum uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public static Point CursorPosition
        {
            get
            {
                var P = new Point();
                GetCursorPos(ref P);
                return P;
            }
        }
    }
}
