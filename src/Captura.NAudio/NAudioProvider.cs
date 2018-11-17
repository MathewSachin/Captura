using System;
using Captura.Audio;
using NAudio.Wave;
using WaveFormat = Captura.Audio.WaveFormat;

namespace Captura.NAudio
{
    public abstract class NAudioProvider : IAudioProvider
    {
        readonly IWaveIn _waveIn;

        protected NAudioProvider(IWaveIn WaveIn)
        {
            _waveIn = WaveIn;

            _waveIn.DataAvailable += (S, E) =>
            {
                DataAvailable?.Invoke(this, new DataAvailableEventArgs(E.Buffer, E.BytesRecorded));
            };
        }

        public void Dispose()
        {
            _waveIn.Dispose();
        }

        public WaveFormat WaveFormat { get; }

        public void Start()
        {
            _waveIn.StartRecording();
        }

        public void Stop()
        {
            _waveIn.StopRecording();
        }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}