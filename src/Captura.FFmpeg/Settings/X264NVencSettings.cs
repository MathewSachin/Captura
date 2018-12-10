using System.Collections.Generic;
using System.Linq;

namespace Captura
{
    public class X264NVencSettings : PropertyStore
    {
        /// <summary>
        /// fast is recommended
        /// </summary>
        public static IEnumerable<string> Presets { get; } = new[] { "slow", "medium", "fast", "ll" };

        public string Preset
        {
            get => Get("fast");
            set
            {
                if (Presets.Contains(value))
                {
                    Set(value);
                }
            }
        }

        public static IEnumerable<string> PixelFormats { get; } = new[] { "yuv420p", "yuv444p" };

        public string PixelFormat
        {
            get => Get("yuv420p");
            set
            {
                if (PixelFormats.Contains(value))
                {
                    Set(value);
                }
            }
        }
    }
}