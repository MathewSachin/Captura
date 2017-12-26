using System.Runtime.InteropServices;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    interface IMediaControl
    {
        [PreserveSig]
        int Run();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int GetState(int MsTimeout, out int Pfs);

        [PreserveSig]
        int RenderFile(string StrFilename);

        [PreserveSig]
        int AddSourceFilter([In] string StrFilename, [Out, MarshalAs(UnmanagedType.IDispatch)] out object PpUnk);

        [PreserveSig]
        int get_FilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object PpUnk);

        [PreserveSig]
        int get_RegFilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object PpUnk);

        [PreserveSig]
        int StopWhenReady();
    }
}
