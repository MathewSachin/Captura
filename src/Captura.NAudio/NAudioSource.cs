using System.Collections.Generic;
using NAudio.CoreAudioApi;

namespace Captura.Audio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NAudioSource : IAudioSource
    {
        public string Name { get; } = "NAudio";

        public IEnumerable<IAudioItem> Microphones
        {
            get
            {
                using var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                foreach (var device in devices)
                {
                    yield return new NAudioItem(device, false);
                }
            }
        }

        public IAudioItem DefaultMicrophone => NAudioDefaultItem.DefaultMicrophone;

        public IEnumerable<IAudioItem> Speakers
        {
            get
            {
                using var enumerator = new MMDeviceEnumerator();
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                foreach (var device in devices)
                {
                    yield return new NAudioItem(device, true);
                }
            }
        }

        public IAudioItem DefaultSpeaker => NAudioDefaultItem.DefaultSpeaker;

        public void Dispose() { }

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
