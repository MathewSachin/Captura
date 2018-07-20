using System.Linq;
using Captura.Audio;
using ManagedBass;

namespace Captura.Models
{
    /// <summary>
    /// ManagedBass Audio Source.
    /// Use <see cref="Available"/> to check if all dependencies are present.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BassAudioSource : AudioSource
    {
        readonly AudioSettings _settings;

        public BassAudioSource(AudioSettings Settings)
        {
            _settings = Settings;

            // Initialises Default Playback Device.
            Bass.Init();

            // Enable Loopback Recording.
            Bass.Configure(Configuration.LoopbackRecording, true);

            Refresh();
        }

        static bool AllExist(params string[] Paths)
        {
            return Paths.All(ServiceProvider.FileExists);
        }

        // Check if all BASS dependencies are present
        public static bool Available { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

        public override IAudioProvider GetMixedAudioProvider()
        {
            return new MixedAudioProvider(AvailableRecordingSources
                .Concat(AvailableLoopbackSources)
                .Cast<BassItem>(),
                !_settings.PlaybackRecordingRealTime
                );
        }

        public override IAudioProvider[] GetMultipleAudioProviders()
        {
            return AvailableRecordingSources.Where(M => M.Active)
                .Concat(AvailableLoopbackSources.Where(M => M.Active))
                .Cast<BassItem>()
                .Select(M => new BassAudioProvider(M))
                .ToArray<IAudioProvider>();
        }

        protected override void OnRefresh()
        {
            for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsLoopback)
                    LoopbackSources.Add(new BassItem(i, info.Name));
                else RecordingSources.Add(new BassItem(i, info.Name));
            }
        }

        /// <summary>
        /// Frees all BASS devices.
        /// </summary>
        public override void Dispose()
        {
            for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsInitialized)
                {
                    Bass.CurrentRecordingDevice = i;
                    Bass.RecordFree();
                }
            }

            for (var i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsInitialized)
                {
                    Bass.CurrentDevice = i;
                    Bass.Free();
                }
            }
        }
    }
}