using System.Runtime.InteropServices;
#pragma warning disable 649
#pragma warning disable 169

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    class VideoInfoHeader
    {
        public RECT SrcRect;
        public RECT TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;
    }
}
