using System;
using System.Runtime.InteropServices;

namespace NWaveIn
{
    /// <summary>
    /// MME Wave function interop
    /// </summary>
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
            /*
            WAVE_FORMAT_QUERY = 1,
            WAVE_MAPPED = 4,
            WAVE_FORMAT_DIRECT = 8*/
        }

        //public const int TIME_MS = 0x0001;  // time in milliseconds 
        //public const int TIME_SAMPLES = 0x0002;  // number of wave samples 
        //public const int TIME_BYTES = 0x0004;  // current byte offset 

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

            /// <summary>
            /// WOM_CLOSE
            /// </summary>
            WaveOutClose = 0x3BC,
            /// <summary>
            /// WOM_DONE
            /// </summary>
            WaveOutDone = 0x3BD,
            /// <summary>
            /// WOM_OPEN
            /// </summary>
            WaveOutOpen = 0x3BB
        }

        // use the userdata as a reference
        // WaveOutProc http://msdn.microsoft.com/en-us/library/dd743869%28VS.85%29.aspx
        // WaveInProc http://msdn.microsoft.com/en-us/library/dd743849%28VS.85%29.aspx
        public delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved);

        [DllImport("winmm.dll")]
        public static extern Int32 mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] String s, int flags);

        [DllImport("winmm.dll")]
        public static extern Int32 waveOutGetNumDevs();
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPrepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutUnprepareHeader(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutWrite(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);
        
        // http://msdn.microsoft.com/en-us/library/dd743866%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);
        [DllImport("winmm.dll", EntryPoint = "waveOutOpen")]
        public static extern MmResult waveOutOpenWindow(out IntPtr hWaveOut, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);
        
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutReset(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutClose(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPause(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutRestart(IntPtr hWaveOut);

        // http://msdn.microsoft.com/en-us/library/dd743863%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, out MmTime mmTime, int uSize);

        // http://msdn.microsoft.com/en-us/library/dd743874%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);
        
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);
        
        [DllImport("winmm.dll")]
        public static extern Int32 waveInGetNumDevs();

        // http://msdn.microsoft.com/en-us/library/dd743841%28VS.85%29.aspx
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult waveInGetDevCaps(IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);
        
        // http://msdn.microsoft.com/en-us/library/dd743838%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, WaveHeader pwh, int cbwh);
        [DllImport("winmm.dll")]
        public static extern MmResult waveInClose(IntPtr hWaveIn);
        
        // http://msdn.microsoft.com/en-us/library/dd743847%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInOpen(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        [DllImport("winmm.dll", EntryPoint = "waveInOpen")]
        public static extern MmResult waveInOpenWindow(out IntPtr hWaveIn, IntPtr uDeviceID, WaveFormat lpFormat, IntPtr callbackWindowHandle, IntPtr dwInstance, WaveInOutOpenFlags dwFlags);

        // http://msdn.microsoft.com/en-us/library/dd743848%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInPrepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);
        
        [DllImport("winmm.dll")]
        public static extern MmResult waveInUnprepareHeader(IntPtr hWaveIn, WaveHeader lpWaveInHdr, int uSize);

        // http://msdn.microsoft.com/en-us/library/dd743850%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInReset(IntPtr hWaveIn);

        // http://msdn.microsoft.com/en-us/library/dd743851%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInStart(IntPtr hWaveIn);

        // http://msdn.microsoft.com/en-us/library/dd743852%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInStop(IntPtr hWaveIn);

    }
}
