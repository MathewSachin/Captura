using System.Runtime.InteropServices;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    struct PinInfo
    {
        public IBaseFilter filter;
        public PinDirection dir;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
    }
}
