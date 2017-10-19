using System;
using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("55272A00-42CB-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPropertyBag
    {
        [PreserveSig]
        int Read([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, [In, Out, MarshalAs(UnmanagedType.Struct)] ref object pVar, IntPtr pErrorLog);

        [PreserveSig]
        int Write([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, [In, MarshalAs(UnmanagedType.Struct)] ref object pVar);
    }
}