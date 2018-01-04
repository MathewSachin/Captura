using System;
using System.Runtime.InteropServices;
// ReSharper disable NotAccessedField.Global
#pragma warning disable 414
#pragma warning disable 169

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
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