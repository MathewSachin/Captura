using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Captura.Webcam
{
    [ComVisible(true), ComImport, Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IEnumFilters
    {
        [PreserveSig]
        int Next([In] uint cFilters, out IBaseFilter x, [Out] out uint pcFetched);

        [PreserveSig]
        int Skip([In] int cFilters);

        void Reset();

        void Clone([Out] out IEnumFilters ppEnum);
    }
}