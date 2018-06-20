using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Global

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("0579154A-2B53-4994-B0D0-E773148EFF85"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ISampleGrabberCB
    {
        [PreserveSig]
        int SampleCB(double sampleTime, IMediaSample pSample);

        [PreserveSig]
        int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen);
    }
}