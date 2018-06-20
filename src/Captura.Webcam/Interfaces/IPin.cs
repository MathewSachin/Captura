using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPin
    {
        [PreserveSig]
        int Connect([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int ReceiveConnection([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int ConnectedTo([Out] out IPin ppPin);

        [PreserveSig]
        int ConnectionMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int QueryPinInfo([Out] out PinInfo pInfo);

        [PreserveSig]
        int QueryDirection(out PinDirection pPinDir);

        [PreserveSig]
        int QueryId([Out, MarshalAs(UnmanagedType.LPWStr)] out string Id);

        [PreserveSig]
        int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int EnumMediaTypes(IntPtr ppEnum);

        [PreserveSig]
        int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);

        [PreserveSig]
        int EndOfStream();

        [PreserveSig]
        int BeginFlush();

        [PreserveSig]
        int EndFlush();

        [PreserveSig]
        int NewSegment(long tStart, long tStop, double dRate);
    }
}