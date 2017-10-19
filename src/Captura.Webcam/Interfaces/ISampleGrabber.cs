using System;
using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ISampleGrabber
    {
        [PreserveSig]
        int SetOneShot([In, MarshalAs(UnmanagedType.Bool)] bool oneShot);

        [PreserveSig]
        int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetConnectedMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int SetBufferSamples([In, MarshalAs(UnmanagedType.Bool)] bool bufferThem);

        [PreserveSig]
        int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);

        [PreserveSig]
        int GetCurrentSample(IntPtr ppSample);

        [PreserveSig]
        int SetCallback(ISampleGrabberCB pCallback, int whichMethodToCallback);
    }
}