using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
// ReSharper disable InconsistentNaming

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator([In] ref Guid pType, [Out] out IEnumMoniker ppEnumMoniker, [In] int dwFlags);
    }
}