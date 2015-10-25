using NAudio.Wave;

namespace Captura
{
    class SilenceProvider : IWaveProvider
    {
        public SilenceProvider(WaveFormat wf) { this.WaveFormat = wf; }

        public int Read(byte[] buffer, int offset, int count)
        {
            buffer.Initialize();
            return count;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}