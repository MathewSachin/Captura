using System.Runtime.InteropServices;

namespace Captura.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public class PackedLong
    {
        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public int Low;

        [FieldOffset(sizeof(int))]
        public int High;
    }
}