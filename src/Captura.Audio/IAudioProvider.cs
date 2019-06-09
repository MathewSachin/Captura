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
        /// Indicates recorded data is available.
        /// </summary>
        event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}
