using System;
using NAudio.Wave;
using Screna.Audio;
using WaveFormat = Screna.Audio.WaveFormat;
using NWaveFormat = NAudio.Wave.WaveFormat;

namespace Screna.NAudio
{
    public class WaveInProvider : IAudioProvider
    {
        readonly WaveInEvent _waveInEvent;

        public WaveInProvider(WaveInDevice Device)
        {
            _waveInEvent = new WaveInEvent
            {
                DeviceNumber = Device.DeviceNumber,
                BufferMilliseconds = 100,
                NumberOfBuffers = 3,
                WaveFormat = new NWaveFormat(8000, 16, 1)
            };

            IsSynchronizable = false;
            WaveFormat = new WaveFormat(8000, 16, 1);

            Setup();
        }

        public WaveInProvider(WaveInDevice Device, int FrameRate, WaveFormat Wf)
        {
            _waveInEvent = new WaveInEvent
            {
                DeviceNumber = Device.DeviceNumber,
                BufferMilliseconds = (int)Math.Ceiling(1000 / (decimal)FrameRate),
                NumberOfBuffers = 3,
                WaveFormat = new NWaveFormat(Wf.SampleRate, Wf.BitsPerSample, Wf.Channels)
            };

            IsSynchronizable = true;
            WaveFormat = Wf;

            Setup();
        }

        void Setup()
        {
            _waveInEvent.RecordingStopped += (Sender, Args) => RecordingStopped?.Invoke(this, new EndEventArgs(Args.Exception));

            _waveInEvent.DataAvailable += (Sender, Args) => DataAvailable?.Invoke(this, new DataAvailableEventArgs(Args.Buffer, Args.BytesRecorded));
        }

        public bool IsSynchronizable { get; }

        public WaveFormat WaveFormat { get; }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
        public event EventHandler<EndEventArgs> RecordingStopped;

        public void Dispose() => _waveInEvent?.Dispose();

        public void Start() => _waveInEvent?.StartRecording();

        public void Stop() => _waveInEvent?.StopRecording();
    }
}
