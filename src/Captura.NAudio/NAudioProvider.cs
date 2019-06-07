using System;
using NAudio.Wave;
using Wf = NAudio.Wave.WaveFormat;
using WfEnc = NAudio.Wave.WaveFormatEncoding;

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

            var wf = WaveIn.WaveFormat;
            NAudioWaveFormat = wf;

            WaveFormat = wf.Encoding == WfEnc.IeeeFloat
                ? WaveFormat.CreateIeeeFloatWaveFormat(wf.SampleRate, wf.Channels)
                : new WaveFormat(wf.SampleRate, wf.BitsPerSample, wf.Channels);
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