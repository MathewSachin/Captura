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

        int Read(byte[] Buffer, int Offset, int Length);
    }
}
