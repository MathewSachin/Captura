using System.Runtime.InteropServices;

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    class OptInt64
    {
        public OptInt64(long value)
        {
            Value = value;
        }

        public long Value;
    }
}