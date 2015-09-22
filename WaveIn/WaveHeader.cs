using System;
using System.Runtime.InteropServices;

namespace Captura
{
    /// <summary>
    /// WaveHeader interop structure (WAVEHDR)
    /// http://msdn.microsoft.com/en-us/library/dd743837%28VS.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    class WaveHeader
    {
        /// <summary>pointer to locked data buffer (lpData)</summary>
        public IntPtr dataBuffer;
        /// <summary>length of data buffer (dwBufferLength)</summary>
        public int bufferLength;
        /// <summary>used for input only (dwBytesRecorded)</summary>
        public int bytesRecorded;
        /// <summary>for client's use (dwUser)</summary>
        public IntPtr userData;
        /// <summary>assorted flags (dwFlags)</summary>
        public WaveHeaderFlags flags;
        /// <summary>loop control counter (dwLoops)</summary>
        public int loops;
        /// <summary>PWaveHdr, reserved for driver (lpNext)</summary>
        public IntPtr next;
        /// <summary>reserved for driver</summary>
        public IntPtr reserved;
    }
}
