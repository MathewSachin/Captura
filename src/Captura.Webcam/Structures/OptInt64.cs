using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global

namespace Captura.Webcam
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once ClassNeverInstantiated.Global
    class OptInt64
    {
        public OptInt64(long Value)
        {
            this.Value = Value;
        }

        public long Value;
    }
}