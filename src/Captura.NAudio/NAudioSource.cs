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
    public class NAudioSource : AudioSource
    {
        public NAudioSource()
        {
            Refresh();
        }

        protected override void OnRefresh()
        {
            var enumerator = new MMDeviceEnumerator();

            var loopbackDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var loopback in loopbackDevs)
            {
                RecordingSources.Add(new NAudioItem(loopback, true));
            }

            var recordingDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var recording in recordingDevs)
            {
                RecordingSources.Add(new NAudioItem(recording, false));
            }
        }

        public override IAudioProvider GetMixedAudioProvider()
        {
            var rec = AvailableRecordingSources
                .Where(M => M.Active)
                .Where(M => !M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiCaptureProvider(M.Device))
                .Cast<NAudioProvider>();

            var loop = AvailableRecordingSources
                .Where(M => M.Active)
                .Where(M => M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiLoopbackCaptureProvider(M.Device))
                .Cast<NAudioProvider>();

            return new MixedAudioProvider(rec.Concat(loop));
        }

        public override IAudioProvider[] GetMultipleAudioProviders()
        {
            var rec = AvailableRecordingSources
                .Where(M => M.Active)
                .Where(M => !M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiCaptureProvider(M.Device))
                .Cast<IAudioProvider>();

            var loop = AvailableRecordingSources
                .Where(M => M.Active)
                .Where(M => M.IsLoopback)
                .Cast<NAudioItem>()
                .Select(M => new WasapiLoopbackCaptureProvider(M.Device))
                .Cast<IAudioProvider>();

            return rec.Concat(loop).ToArray();
        }

        public override string Name { get; } = "NAudio";
    }
}
