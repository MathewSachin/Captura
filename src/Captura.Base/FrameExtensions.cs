using Captura.Models;

namespace Captura
{
    public static class FrameExtensions
    {
        public static IBitmapFrame Unwrap(this IBitmapFrame Frame)
        {
            while (Frame is IFrameWrapper wrapper)
            {
                Frame = wrapper.Frame;
            }

            return Frame;
        }
    }
}
