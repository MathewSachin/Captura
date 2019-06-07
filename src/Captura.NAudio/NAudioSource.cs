using System.Collections.Generic;
using NAudio.CoreAudioApi;

namespace Captura.Audio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// Fallback to NAudio when BASS is not available.
    /// </summary>
    /// <remarks>
    /// The audio device must support WASAPI Shared mode in 32-bit floating-point, 44100 Hz, Stereo.
    /// Recording to separate audio files is working.
    /// </remarks>
    public class NAudioSource : IAudioSource
    {
        public string Name { get; } = "NAudio";

        public IEnumerable<IAudioItem> Microphones
        {
            get
            {
                using (var enumerator = new MMDeviceEnumerator())
                {
                    var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                    foreach (var device in devices)
                    {
                        yield return new NAudioItem(device, false);
                    }
                }
            }
        }

        public IAudioItem DefaultMicrophone => throw new System.NotImplementedException();

        public IEnumerable<IAudioItem> Speakers
        {
            get
            {
                using (var enumerator = new MMDeviceEnumerator())
                {
                    var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                    foreach (var device in devices)
                    {
                        yield return new NAudioItem(device, true);
                    }
                }
            }
        }

        public IAudioItem DefaultSpeaker => throw new System.NotImplementedException();

        public void Dispose() { }

        public IAudioProvider GetAudioProvider(IAudioItem Microphone, IAudioItem Speaker)
        {
            if (Microphone == null && Speaker is NAudioItem speakerItem)
            {
                return new WasapiLoopbackCaptureProvider(speakerItem.Device);
            }

            if (Microphone is NAudioItem micItem && Speaker == null)
            {
                return new WasapiCaptureProvider(micItem.Device);
            }

            if (Microphone is NAudioItem a && Speaker is NAudioItem b)
            {
                return new MixedAudioProvider(new NAudioProvider[]
                {
                    new WasapiCaptureProvider(a.Device),
                    new WasapiLoopbackCaptureProvider(b.Device)
                });
            }

            return null;
        }
    }
}
