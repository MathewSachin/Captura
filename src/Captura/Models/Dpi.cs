using System.Windows.Interop;

namespace Captura
{
    /// <summary>
    /// Provides DPI scaling factor.
    /// Only needs to be used when dealing with WPF since their sizes are specified in Device Independent Pixels.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Dpi
    {
        static Dpi()
        {
            using var src = new HwndSource(new HwndSourceParameters());
            if (src.CompositionTarget != null)
            {
                var matrix = src.CompositionTarget.TransformToDevice;

                X = (float) matrix.M11;
                Y = (float) matrix.M22;
            }
        }

        public static float X { get; }

        public static float Y { get; }
    }
}
