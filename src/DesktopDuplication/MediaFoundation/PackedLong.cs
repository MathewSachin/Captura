﻿using System.Runtime.InteropServices;

namespace DesktopDuplication
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