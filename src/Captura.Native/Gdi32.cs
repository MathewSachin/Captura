using System;
using System.Runtime.InteropServices;

namespace Captura.Native
{
    public static class Gdi32
    {
        const string DllName = "gdi32.dll";

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr HObject);
    }
}