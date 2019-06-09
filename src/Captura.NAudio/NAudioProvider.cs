using System;
using NAudio.Wave;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.Audio
{
    abstract class NAudioProvider : IAudioProvider
    {
        readonly IWaveIn _waveIn;

        protected NAudioProvider(IWaveIn WaveIn)
        {
            _waveIn = WaveIn;

            _waveIn.DataAvailable += (S, E) =>
            {
                DataAvailable?.Invoke(this, new DataAvailableEventArgs(E.Buffer, E.BytesRecorded));
            };

            NAudioWaveFormat = WaveIn.WaveFormat;
            WaveFormat = WaveIn.WaveFormat.ToCaptura();
        }

        public virtual void Dispose()
        {
            _waveIn.Dispose();
        }

        public WaveFormat WaveFormat { get; }

        public Wf NAudioWaveFormat { get; }

        public virtual void Start()
        {
            _waveIn.StartRecording();
        }

        public virtual void Stop()
        {
            _waveIn.StopRecording();
        }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}