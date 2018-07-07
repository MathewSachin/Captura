using System.Runtime.InteropServices;

namespace Captura.Native
{
    public static class ShCore
    {
        const string DllName = "shcore.dll";

        [DllImport(DllName)]
        public static extern int SetProcessDpiAwareness(ProcessDPIAwareness Value);
    }
}