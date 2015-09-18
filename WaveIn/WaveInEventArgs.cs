using System;

namespace NWaveIn
{
    /// <summary>
    /// Event Args for WaveInStream event
    /// </summary>
    public class WaveInEventArgs : EventArgs
    {
        byte[] buffer;
        int bytes;

        /// <summary>
        /// Creates new WaveInEventArgs
        /// </summary>
        public WaveInEventArgs(byte[] buffer, int bytes)
        {
            this.buffer = buffer;
            this.bytes = bytes;
        }

        /// <summary>
        /// Buffer containing recorded data. Note that it might not be completely
        /// full. <seealso cref="BytesRecorded"/>
        /// </summary>
        public byte[] Buffer { get { return buffer; } }

        /// <summary>
        /// The number of recorded bytes in Buffer. <seealso cref="Buffer"/>
        /// </summary>
        public int BytesRecorded { get { return bytes; } }
    }
}
