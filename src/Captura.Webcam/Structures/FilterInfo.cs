using System.Runtime.InteropServices;
#pragma warning disable 169

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    // ReSharper disable once ClassNeverInstantiated.Global
    class FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;

        [MarshalAs(UnmanagedType.IUnknown)]
        public object pUnk;
    }
}