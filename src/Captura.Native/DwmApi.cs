using System;
using System.Runtime.InteropServices;

namespace Captura.Native
{
    public class DwmApi
    {
        const string DllName = "dwmapi.dll";

        [DllImport(DllName)]
        public static extern int DwmGetWindowAttribute(IntPtr Window, int Attribute, out bool Value, int Size);
    }
}