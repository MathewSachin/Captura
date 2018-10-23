using System;

namespace Captura.Audio
{
    /// <summary>
    /// Encodes Audio into an audio file.
    /// </summary>
    public interface IAudioFileWriter : IDisposable
    {
        /// <summary>
        /// Writes to file.
        /// </summary>
        void Write(byte[] Data, int Offset, int Count);

        /// <summary>
        /// Writes all buffered data to file.
        /// </summary>
        void Flush();
    }
}
