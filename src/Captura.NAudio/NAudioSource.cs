using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace Captura.Audio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NAudioSource : IAudioSource
    {
        MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        NAudioNotificationClient _notificationClient = new NAudioNotificationClient();

        public event Action DevicesUpdated;

        public NAudioSource()
        {
            _notificationClient.DevicesUpdated += () => DevicesUpdated?.Invoke();

            _deviceEnumerator.RegisterEndpointNotificationCallback(_notificationClient);
        }

        public string Name { get; } = "NAudio";

        public IEnumerable<IAudioItem> Microphones
        {
            get
            {
                var devices = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                foreach (var device in devices)
                {
                    yield return new NAudioItem(device, false);
                }
            }
        }

        const int DeviceNotFound = unchecked((int)0x80070490);

        public IAudioItem DefaultMicrophone
        {
            get
            {
                try
                {
                    return NAudioItem.DefaultMicrophone;
                }
                // Default mic does not exist
                catch (COMException e) when (e.HResult == DeviceNotFound)
                {
                    return null;
                }
            }
        }

        public IEnumerable<IAudioItem> Speakers
        {
            get
            {
                var devices = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                foreach (var device in devices)
                {
                    yield return new NAudioItem(device, true);
                }
            }
        }

        public IAudioItem DefaultSpeaker
        {
            get
            {
                try
                {
                    return NAudioItem.DefaultSpeaker;
                }
                // Default speaker does not exist
                catch (COMException e) when (e.HResult == DeviceNotFound)
                {
                    return null;
                }
            }
        }

        public void Dispose()
        {
            _deviceEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
            _notificationClient = null;

            _deviceEnumerator.Dispose();
            _deviceEnumerator = null;
        }

        public IAudioProvider GetAudioProvider(IAudioItem Microphone, IAudioItem Speaker)
        {
            switch ((Microphone, Speaker))
            {
                case (null, NAudioItem speaker):
                    return new MixedAudioProvider(new WasapiLoopbackCaptureProvider(speaker.Device));

                case (NAudioItem mic, null):
                    return new MixedAudioProvider(new WasapiCaptureProvider(mic.Device));

                case (NAudioItem mic, NAudioItem speaker):
                    return new MixedAudioProvider(
                        new WasapiCaptureProvider(mic.Device),
                        new WasapiLoopbackCaptureProvider(speaker.Device));

                default:
                    return null;
            }
        }
    }
}
