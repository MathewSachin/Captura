using System.Collections.Generic;
using System.Linq;
using Captura.Audio;
using Captura.Models;
using NAudio.CoreAudioApi;

namespace Captura.NAudio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// Fallback to NAudio when BASS is not available.
    /// </summary>
    /// <remarks>
    /// The audio device must support WASAPI Shared mode in 32-bit floating-point, 44100 Hz, Stereo.
    /// Recording to separate audio files is working.
    /// Audio mixing works but changing sources during recording is not supported.
    /// </remarks>
    public class NAudioSource : IAudioSource
    {
        public IEnumerable<IAudioItem> GetSources()
        {
            var enumerator = new MMDeviceEnumerator();

            var loopbackDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var loopback in loopbackDevs)
            {
                yield return new NAudioItem(loopback, true);
            }

            var recordingDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var recording in recordingDevs)
            {
                yield return new NAudioItem(recording, false);
            }
        }

        public IAudioProvider GetMixedAudioProvider(IEnumerable<IIsActive<IAudioItem>> AudioItems)
        {
            var rec = AudioItems
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => !M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiCaptureProvider(M.Device))
                .Cast<NAudioProvider>();

            var loop = AudioItems
                .Where(M => M.IsActive)
                .Select(M => M.Item)
                .Where(M => M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiLoopbackCaptureProvider(M.Device))
                .Cast<NAudioProvider>();

            return new MixedAudioProvider(rec.Concat(loop));
        }

        public IAudioProvider GetAudioProvider(IAudioItem AudioItem)
        {
            if (AudioItem is NAudioItem item)
            {
                if (item.IsLoopback)
                    return new WasapiLoopbackCaptureProvider(item.Device);

                return new WasapiCaptureProvider(item.Device);
            }

            return null;
        }

        public string Name { get; } = "NAudio";

        public bool CanChangeSourcesDuringRecording => false;

        public void Dispose() { }
    }
}
