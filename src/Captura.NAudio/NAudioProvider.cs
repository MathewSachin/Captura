using NAudio.Wave;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.Audio
{
    abstract class NAudioProvider
    {
        public IWaveIn WaveIn { get; }

        protected NAudioProvider(IWaveIn WaveIn)
        {
            this.WaveIn = WaveIn;

            NAudioWaveFormat = WaveIn.WaveFormat;
            WaveFormat = WaveIn.WaveFormat.ToCaptura();
        }

        public virtual void Dispose()
        {
            WaveIn.Dispose();
        }

        public WaveFormat WaveFormat { get; }

        public Wf NAudioWaveFormat { get; }

        public virtual void Start()
        {
            WaveIn.StartRecording();
        }

        public virtual void Stop()
        {
            WaveIn.StopRecording();
        }
    }
}