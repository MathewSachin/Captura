using NAudio.Wave;

namespace Screna.NAudio
{
    class SilenceProvider : IWaveProvider
    {
        public WaveFormat WaveFormat => new WaveFormat();

        public int Read(byte[] buffer, int offset, int count)
        {
            var n = offset + count;

            for (var i = offset; i < n; ++i)
                buffer[i] = 0;

            return count;
        }
    }
}