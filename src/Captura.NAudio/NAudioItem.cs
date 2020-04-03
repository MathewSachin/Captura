using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Captura.Audio
{
    class NAudioItem : IAudioItem
    {
        public MMDevice Device { get; }

        public bool IsLoopback { get; }

        AudioClient _audioClient;

        public void StartListeningForPeakLevel()
        {
            if (_audioClient != null)
                return;

            // Peak Level is available for recording devices only when they are active
            if (IsLoopback)
                return;

            _audioClient = Device.AudioClient;
            _audioClient.Initialize(AudioClientShareMode.Shared,
                AudioClientStreamFlags.None,
                100,
                100,
                _audioClient.MixFormat,
                Guid.Empty);

            _audioClient.Start();
        }

        public void StopListeningForPeakLevel()
        {
            if (_audioClient == null)
                return;

            _audioClient.Stop();
            _audioClient.Dispose();
            _audioClient = null;

            _audioClient = null;
        }

        public string Name { get; }

        public NAudioItem(MMDevice Device, bool IsLoopback)
            : this(Device, Device.FriendlyName, IsLoopback)
        {
        }

        NAudioItem(MMDevice Device, string Name, bool IsLoopback)
        {
            this.Device = Device;
            this.IsLoopback = IsLoopback;
            this.Name = Name;
        }

        const string DefaultDeviceName = "Default";

        public static NAudioItem DefaultMicrophone => new NAudioItem(
            WasapiCapture.GetDefaultCaptureDevice(),
            DefaultDeviceName,
            false);

        public static NAudioItem DefaultSpeaker => new NAudioItem(
            WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice(),
            DefaultDeviceName,
            true);

        public double PeakLevel => Device.AudioMeterInformation.MasterPeakValue;

        public override string ToString() => Name;
        
        public void Dispose()
        {
            StopListeningForPeakLevel();

            // Not disposing the device as it may be in use in a recording.
        }
    }
}