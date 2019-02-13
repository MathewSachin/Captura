using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Captura.Native
{
    static class User32
    {
        const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool SetProcessDPIAware();
    }
}
