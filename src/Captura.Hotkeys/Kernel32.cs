using System.Runtime.InteropServices;

namespace Captura.Native
{
    static class Kernel32
    {
        const string DllName = "kernel32";

        [DllImport(DllName)]
        public static extern ushort GlobalAddAtom(string Text);

        [DllImport(DllName)]
        public static extern ushort GlobalDeleteAtom(ushort Atom);
    }
}