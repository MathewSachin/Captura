using System;
using System.Drawing;
using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Screna.Native
{
    [StructLayout(LayoutKind.Sequential)]
    struct IconInfo
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CursorInfo
    {
        public int cbSize;
        public int flags;
        public IntPtr hCursor;
        public Point ptScreenPos;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int Dimension)
        {
            Left = Top = Right = Bottom = Dimension;
        }
    }
}