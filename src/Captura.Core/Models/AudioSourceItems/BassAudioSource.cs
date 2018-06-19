using System.Linq;
using Screna.Audio;
using ManagedBass;

namespace Captura.Models
{
    /// <summary>
    /// ManagedBass Audio Source.
    /// Use <see cref="Available"/> to check if all dependencies are present.
    /// </summary>
    public class BassAudioSource : AudioSource
    {
        public BassAudioSource()
        {
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

        public override IAudioProvider GetAudioProvider(int FrameRate)
        {
            return new MixedAudioProvider(AvailableRecordingSources
                .Concat(AvailableLoopbackSources)
                .Cast<BassItem>(),
                FrameRate,
                !ServiceProvider.Get<Settings>().Audio.PlaybackRecordingRealTime);
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