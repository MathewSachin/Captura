using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Screna.Audio;
using WaveFormat = Screna.Audio.WaveFormat;

namespace Screna.NAudio
{
    public class LoopbackProvider : IAudioProvider
    {
        readonly WasapiOut _silenceOut;
        readonly WasapiLoopbackCapture _capture;
        static readonly MMDeviceEnumerator DeviceEnumerator = new MMDeviceEnumerator();

        public static MMDevice DefaultDevice => DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        public static IEnumerable<MMDevice> EnumerateDevices() => DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        public LoopbackProvider(MMDevice Device = null, bool IncludeSilence = true)
        {
            if (Device == null)
                Device = DefaultDevice;

            _capture = new WasapiLoopbackCapture(Device);

            _capture.DataAvailable += (Sender, Args) => DataAvailable?.Invoke(this, new DataAvailableEventArgs(Args.Buffer, Args.BytesRecorded));
            _capture.RecordingStopped += (Sender, Args) => RecordingStopped?.Invoke(this, new EndEventArgs(Args.Exception));

            var mixFormat = _capture.WaveFormat;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(mixFormat.SampleRate, mixFormat.Channels);

            if (!IncludeSilence)
                return;

            _silenceOut = new WasapiOut(Device, AudioClientShareMode.Shared, false, 100);
            _silenceOut.Init(new SilenceProvider());
        }

        public WaveFormat WaveFormat { get; }

        public void Start()
        {
            _silenceOut.Play();
            _capture.StartRecording();
        }

        public void Stop()
        {
            _capture.StopRecording();
            _silenceOut.Stop();
        }

        public bool IsSynchronizable => false;

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
        public event EventHandler<EndEventArgs> RecordingStopped;

        public void Dispose()
        {
            _capture?.Dispose();
            _silenceOut?.Dispose();
        }
    }
}
