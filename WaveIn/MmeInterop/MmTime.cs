using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NWaveIn
{
    // http://msdn.microsoft.com/en-us/library/dd757347(v=VS.85).aspx
    [StructLayout(LayoutKind.Explicit)]
    struct MmTime
    {
        public const int TIME_MS = 0x0001;
        public const int TIME_SAMPLES = 0x0002;
        public const int TIME_BYTES = 0x0004;

        [FieldOffset(0)]
        public UInt32 wType;
        [FieldOffset(4)]
        public UInt32 ms;
        [FieldOffset(4)]
        public UInt32 sample;
        [FieldOffset(4)]
        public UInt32 cb;
        [FieldOffset(4)]
        public UInt32 ticks;
        [FieldOffset(4)]
        public Byte smpteHour;
        [FieldOffset(5)]
        public Byte smpteMin;
        [FieldOffset(6)]
        public Byte smpteSec;
        [FieldOffset(7)]
        public Byte smpteFrame;
        [FieldOffset(8)]
        public Byte smpteFps;
        [FieldOffset(9)]
        public Byte smpteDummy;
        [FieldOffset(10)]
        public Byte smptePad0;
        [FieldOffset(11)]
        public Byte smptePad1;
        [FieldOffset(4)]
        public UInt32 midiSongPtrPos;
    }
}
