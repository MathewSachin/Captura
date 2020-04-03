using SharpDX;
using SharpDX.DXGI;

namespace Captura.Windows.DesktopDuplication
{
    public class AcquireResult
    {
        public AcquireResult(Result Result)
        {
            this.Result = Result;
        }

        public AcquireResult(Result Result, OutputDuplicateFrameInformation FrameInfo, Resource DesktopResource)
        {
            this.Result = Result;
            this.FrameInfo = FrameInfo;
            this.DesktopResource = DesktopResource;
        }

        public Result Result { get; }

        public OutputDuplicateFrameInformation FrameInfo { get; }

        public Resource DesktopResource { get; }
    }
}