using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("56a86897-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IReferenceClock
    {
        [PreserveSig]
        int GetTime(out long pTime);

        [PreserveSig]
        int AdviseTime(long baseTime, long streamTime, IntPtr hEvent, out int pdwAdviseCookie);

        [PreserveSig]
        int AdvisePeriodic(long startTime, long periodTime, IntPtr hSemaphore, out int pdwAdviseCookie);

        [PreserveSig]
        int Unadvise(int dwAdviseCookie);
    }
}