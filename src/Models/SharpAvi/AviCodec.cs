using SharpAvi;

namespace Screna.Avi
{
    /// <summary>
    /// Represents an Avi Codec.
    /// </summary>
    public class AviCodec
    {
        internal FourCC FourCC { get; }

        /// <summary>
        /// Name of the Codec
        /// </summary>
        public string Name { get; }

        internal AviCodec(FourCC FourCC, string Name)
        {
            this.FourCC = FourCC;
            this.Name = Name;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AviCodec"/>.
        /// </summary>
        public AviCodec(string Name)
        {
            FourCC = new FourCC("____");
            this.Name = Name;
        }
        
        /// <summary>
        /// Quality of the encoded Video... 0 to 100 (default is 70) (Not supported by all Codecs). 
        /// </summary>
        public int Quality { get; set; } = 70;

        /// <summary>Identifier used for non-compressed data.</summary>
        public static readonly AviCodec Uncompressed = new AviCodec(new FourCC(0), "Uncompressed");

        /// <summary>Motion JPEG.</summary>
        public static readonly AviCodec MotionJpeg = new AviCodec(new FourCC("MJPG"), "Motion Jpeg");

        /// <summary>Microsoft MPEG-4 V3.</summary>
        public static readonly AviCodec MicrosoftMpeg4V3 = new AviCodec(new FourCC("MP43"), "Microsoft Mpeg-4 v3");

        /// <summary>Microsoft MPEG-4 V2.</summary>
        public static readonly AviCodec MicrosoftMpeg4V2 = new AviCodec(new FourCC("MP42"), "Microsoft Mpeg-4 v2");

        /// <summary>Xvid MPEG-4.</summary>
        public static readonly AviCodec Xvid = new AviCodec(new FourCC("XVID"), "Xvid Mpeg-4");

        /// <summary>DivX MPEG-4.</summary>
        public static readonly AviCodec DivX = new AviCodec(new FourCC("DIVX"), "DivX Mpeg-4");

        /// <summary>x264 H.264/MPEG-4 AVC.</summary>
        public static readonly AviCodec X264 = new AviCodec(new FourCC("X264"), "x264 H.264/Mpeg-4 AVC");
    }
}
