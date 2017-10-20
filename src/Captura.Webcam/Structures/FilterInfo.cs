using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    class FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;

        [MarshalAs(UnmanagedType.IUnknown)]
        public object pUnk;
    }
}