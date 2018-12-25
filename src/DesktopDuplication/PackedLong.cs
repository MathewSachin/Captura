using System.Runtime.InteropServices;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace DesktopDuplication
{
    [StructLayout(LayoutKind.Explicit)]
    struct PackedLong
    {
        [FieldOffset(0)]
        public long Value;

        [FieldOffset(0)]
        public int Low;

        [FieldOffset(sizeof(long) / 2)]
        public int High;
    }
}