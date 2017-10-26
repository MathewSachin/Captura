using System;
using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    class AMMediaType
    {
        public Guid majorType;
        public Guid subType;

        [MarshalAs(UnmanagedType.Bool)]
        public bool fixedSizeSamples;

        [MarshalAs(UnmanagedType.Bool)]
        public bool temporalCompression;

        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr;
        public int formatSize;
        public IntPtr formatPtr;
    }
}