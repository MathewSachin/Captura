using System;
using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    static class Workaround
    {
        /*
        works:
            CoCreateInstance( CLSID_CaptureGraphBuilder2, ..., IID_ICaptureGraphBuilder2, ...);
        doesn't (E_NOTIMPL):
            CoCreateInstance( CLSID_CaptureGraphBuilder2, ..., IID_IUnknown, ...);
        thus .NET 'Activator.CreateInstance' fails
        */

        public static object CreateDsInstance(ref Guid clsid, ref Guid riid)
        {
            const int clsCtxInProc = 0x03;

            var hr = CoCreateInstance(ref clsid, IntPtr.Zero, clsCtxInProc, ref riid, out IntPtr ptrIf);
            if (hr != 0 || ptrIf == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(hr);

            var iu = new Guid("00000000-0000-0000-C000-000000000046");
            Marshal.QueryInterface(ptrIf, ref iu, out IntPtr _);

            var ooo = System.Runtime.Remoting.Services.EnterpriseServicesHelper.WrapIUnknownWithComObject(ptrIf);
            Marshal.Release(ptrIf);
            return ooo;
        }

        [DllImport("ole32.dll")]
        static extern int CoCreateInstance(ref Guid clsid, IntPtr pUnkOuter, int dwClsContext, ref Guid iid, out IntPtr ptrIf);
    }
}
