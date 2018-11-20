using System;
using Captura.Audio;
using NAudio.Wave;
using WaveFormat = Captura.Audio.WaveFormat;
using WaveFormatEncoding = NAudio.Wave.WaveFormatEncoding;

namespace Captura.NAudio
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

            var wf = WaveIn.WaveFormat;

            WaveFormat = wf.Encoding == WaveFormatEncoding.IeeeFloat
                ? WaveFormat.CreateIeeeFloatWaveFormat(wf.SampleRate, wf.Channels)
                : new WaveFormat(wf.SampleRate, wf.BitsPerSample, wf.Channels);
        }

        public virtual void Dispose()
        {
            _waveIn.Dispose();
        }

        public WaveFormat WaveFormat { get; }

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