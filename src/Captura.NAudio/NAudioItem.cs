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

        // Hold a reference to AudioClient
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        readonly AudioClient _audioClient;

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

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public double PeakLevel
        {
            get
            {
                var val = 0.0;

                _syncContext.Send(M => val = Device.AudioMeterInformation.MasterPeakValue, null);

                return val;
            }
        }

        public override string ToString() => Name;
    }
}