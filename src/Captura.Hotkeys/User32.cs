using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Captura.Native
{
    static class User32
    {
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        public static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, uint VirtualKey);
    }
}
