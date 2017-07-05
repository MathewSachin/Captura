﻿using System;

namespace Screna.Audio
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

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        event EventHandler<EndEventArgs> RecordingStopped;
    }
}
