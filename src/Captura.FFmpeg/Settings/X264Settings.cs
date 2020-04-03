using System.Collections.Generic;
using System.Linq;

namespace Captura.FFmpeg
{
    public class X264Settings : PropertyStore
    {
        /// <summary>
        /// ultrafast is recommended
        /// </summary>
        public static IEnumerable<string> Presets { get; } = new[] { "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast" };

        public string Preset
        {
            get => Get("ultrafast");
            set
            {
                if (Presets.Contains(value))
                {
                    Set(value);
                }
            }
        }

        /// <summary>
        /// yuv420p has better compatibility whereas yuv444p has better quality
        /// </summary>
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