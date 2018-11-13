using System.Runtime.InteropServices;
using DirectShowLib;

#pragma warning disable 649
#pragma warning disable 169

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    class VideoInfoHeader
    {
        public DsRect SrcRect;
        public DsRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;
    }
}
