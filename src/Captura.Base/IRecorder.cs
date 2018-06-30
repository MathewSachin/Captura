using System;

namespace Captura
{
    /// <summary>
    /// Carries out the process of recording Audio and/or Video.
    /// </summary>
    public interface IRecorder : IDisposable
    {
        /// <summary>
        /// Start Recording.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop Recording.
        /// </summary>
        void Stop();

        /// <summary>
        /// Fired when an error occurs.
        /// </summary>
        event Action<Exception> ErrorOccurred;
    }
}
