namespace Captura.Audio
{
    /// <summary>
    /// WaveFormat Encoding
    /// </summary>
    public enum WaveFormatEncoding : ushort
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0x0000,

        /// <summary>
        /// Pulse Code Modulation.
        /// </summary>
        Pcm = 0x0001,

        /// <summary>
        /// IEEE Float.
        /// </summary>
        Float = 0x0003,

        /// <summary>
        /// MPEG Layer 3 (MP3).
        /// </summary>
        Mp3 = 0x0055,

        /// <summary>
        /// Wave Format Extensible.
        /// </summary>
        Extensible = 0xFFFE
    }
}
