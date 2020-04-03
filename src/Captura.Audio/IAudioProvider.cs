using System;

namespace Captura.Audio
{
    /// <summary>
    /// Provides Recorded Audio.
    /// </summary>
    public interface IAudioProvider : IDisposable
    {
        /// <summary>
        /// Gets the Recording WaveFormat.
        /// </summary>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Start Recording.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop Recording.
        /// </summary>
        void Stop();

        /// <summary>
        /// Read data into a buffer.
        /// </summary>
        /// <param name="Buffer">Buffer to read data into.</param>
        /// <param name="Offset">Offset from which data should be written to the buffer.</param>
        /// <param name="Length">Number of bytes to write to the buffer.</param>
        /// <returns>Number of bytes read.</returns>
        int Read(byte[] Buffer, int Offset, int Length);
    }
}
