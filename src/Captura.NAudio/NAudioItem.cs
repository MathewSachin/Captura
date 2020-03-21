using System;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Captura.Audio
{
    class NAudioItem : IAudioItem
    {
        public MMDevice Device { get; }

        public bool IsLoopback { get; }

        public string Name { get; }

        public NAudioItem(MMDevice Device, bool IsLoopback)
            : this(Device, Device.FriendlyName, IsLoopback)
        {
        }

        AudioClient _audioClient;

        NAudioItem(MMDevice Device, string Name, bool IsLoopback)
        {
            this.Device = Device;
            this.IsLoopback = IsLoopback;
            this.Name = Name;

            // Peak Level is available for recording devices only when they are active
            if (!IsLoopback)
            {
                _audioClient = Device.AudioClient;
                _audioClient.Initialize(AudioClientShareMode.Shared,
                    AudioClientStreamFlags.None,
                    100,
                    100,
                    _audioClient.MixFormat,
                    Guid.Empty);

                _audioClient.Start();
            }
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
            if (_audioClient == null)
                return;

            _audioClient.Stop();
            _audioClient.Dispose();
            _audioClient = null;

            // Not disposing the device as it may be in use in a recording.
        }
    }
}