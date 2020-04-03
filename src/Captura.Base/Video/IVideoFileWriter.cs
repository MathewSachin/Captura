using System;

namespace Captura.Video
{
    /// <summary>
    /// Creates a video from individual frames and writes them to a file.
    /// </summary>
    public interface IVideoFileWriter : IDisposable
    {
        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        void WriteFrame(IBitmapFrame Image);
        
        /// <summary>
        /// Gets whether audio is supported.
        /// </summary>
        bool SupportsAudio { get; }
                
        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        void WriteAudio(byte[] Buffer, int Offset, int Length);
    }
}
