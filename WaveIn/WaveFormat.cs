using System;
using System.Runtime.InteropServices;

namespace NWaveIn
{
    /// <summary>
    /// Represents a Wave file format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormat
    {
        /// <summary>format type</summary>
        protected ushort waveFormatTag = 1;
        /// <summary>number of channels</summary>
        protected short channels;
        /// <summary>sample rate</summary>
        protected int sampleRate;
        /// <summary>for buffer estimation</summary>
        protected int averageBytesPerSecond;
        /// <summary>block size of data</summary>
        protected short blockAlign;
        /// <summary>number of bits per sample of mono data</summary>
        protected short bitsPerSample;
        /// <summary>number of following bytes</summary>
        protected short extraSize = 0;

        /// <summary>
        /// Creates a new PCM format with the specified sample rate, bit depth and channels
        /// </summary>
        public WaveFormat(int rate = 44100, int bits = 16, int channels = 2)
        {
            if (channels < 1) throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
            // minimum 16 bytes, sometimes 18 for PCM
            this.channels = (short)channels;
            this.sampleRate = rate;
            this.bitsPerSample = (short)bits;
            
            this.blockAlign = (short)(channels * (bits / 8));
            this.averageBytesPerSecond = this.sampleRate * this.blockAlign;
        }
        
        /// <summary>
        /// Returns the number of channels (1=mono,2=stereo etc)
        /// </summary>
        public int Channels { get { return channels; } }

        /// <summary>
        /// Returns the sample rate (samples per second)
        /// </summary>
        public int SampleRate { get { return sampleRate; } }

        /// <summary>
        /// Returns the average number of bytes used per second
        /// </summary>
        public int AverageBytesPerSecond { get { return averageBytesPerSecond; } }

        /// <summary>
        /// Returns the block alignment
        /// </summary>
        public virtual int BlockAlign { get { return blockAlign; } }

        /// <summary>
        /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
        /// Can be 0 for some codecs
        /// </summary>
        public int BitsPerSample { get { return bitsPerSample; } }
    }
}
