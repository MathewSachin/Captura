using SharpAvi;

namespace Captura.SharpAvi
{
    /// <summary>
    /// Represents an Avi Codec.
    /// </summary>
    class AviCodec
    {
        // ReSharper disable once InconsistentNaming
        internal FourCC FourCC { get; }

        /// <summary>
        /// Name of the Codec
        /// </summary>
        public string Name { get; }

        // ReSharper disable once InconsistentNaming
        internal AviCodec(FourCC FourCC, string Name)
        {
            this.FourCC = FourCC;
            this.Name = Name;
        }
        
        /// <summary>
        /// Quality of the encoded Video... 0 to 100 (default is 70) (Not supported by all Codecs). 
        /// </summary>
        public int Quality { get; set; } = 70;

        /// <summary>Identifier used for non-compressed data.</summary>
        public static AviCodec Uncompressed { get; } = new AviCodec(new FourCC(0), "Uncompressed");

        /// <summary>Motion JPEG.</summary>
        public static AviCodec MotionJpeg { get; } = new AviCodec(new FourCC("MJPG"), "Motion Jpeg");

        public static AviCodec Lagarith { get; } = new AviCodec(new FourCC("LAGS"), "Lagarith (Install Manually)");
    }
}
