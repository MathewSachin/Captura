using System;
using System.Runtime.InteropServices;

namespace NWaveIn
{
    class WaveInterop
    {
        [Flags]
        public enum WaveInOutOpenFlags
        {
            /// <summary>
            /// CALLBACK_NULL
            /// No callback
            /// </summary>
            CallbackNull = 0,
            /// <summary>
            /// CALLBACK_FUNCTION
            /// dwCallback is a FARPROC 
            /// </summary>
            CallbackFunction = 0x30000,
            /// <summary>
            /// CALLBACK_EVENT
            /// dwCallback is an EVENT handle 
            /// </summary>
            CallbackEvent = 0x50000,
            /// <summary>
            /// CALLBACK_WINDOW
            /// dwCallback is a HWND 
            /// </summary>
            CallbackWindow = 0x10000,
            /// <summary>
            /// CALLBACK_THREAD
            /// callback is a thread ID 
            /// </summary>
            CallbackThread = 0x20000,
        }

        public enum WaveMessage
        {
            /// <summary>
            /// WIM_OPEN
            /// </summary>
            WaveInOpen = 0x3BE,
            /// <summary>
            /// WIM_CLOSE
            /// </summary>
            WaveInClose = 0x3BF,
            /// <summary>
            /// WIM_DATA
            /// </summary>
            WaveInData = 0x3C0,
        }

        public delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved);
                        
        [DllImport("winmm.dll")]
        public static extern Int32 waveInGetNumDevs();

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult waveInGetDevCaps(IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);
                
        [DllImport("winmm.dll")]
        public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, WaveHeader pwh, int cbwh);
        
        [DllImport("winmm.dll")]
        public static extern MmResult waveInClose(IntPtr hWaveIn);
        
        [DllImport("winmm.dll", EntryPoint = "waveInOpen")]
        public static extern MmResult waveInOpenWindow(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInPrepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);
        
        [DllImport("winmm.dll")]
        public static extern MmResult waveInUnprepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInReset(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInStart(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInStop(IntPtr hWaveIn);
    }
}
