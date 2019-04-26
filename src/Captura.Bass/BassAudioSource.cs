using System.Collections.Generic;
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
    public class BassAudioSource : IAudioSource
    {
        public BassAudioSource()
        {
            // Initialises Default Playback Device.
            Bass.Init();

            // Enable Loopback Recording.
            Bass.Configure(Configuration.LoopbackRecording, true);
        }

        static bool AllExist(params string[] Paths)
        {
            return Paths.All(ServiceProvider.FileExists);
        }

        // Check if all BASS dependencies are present
        public static bool Available { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

        public IAudioProvider GetMixedAudioProvider(IEnumerable<IIsActive<IAudioItem>> AudioItems)
        {
            var bassItems = AudioItems
                .Select(M => (M.Item as BassItem).ToIsActive(M.IsActive));

            return new MixedAudioProvider(bassItems);
        }

        public IAudioProvider GetAudioProvider(IAudioItem AudioItem)
        {
            if (AudioItem is BassItem item)
                return new BassAudioProvider(item);

            return null;
        }

        public IEnumerable<IAudioItem> GetSources()
        {
            for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                yield return new BassItem(i, info.Name, info.IsLoopback);
            }
        }

        /// <summary>
        /// Frees all BASS devices.
        /// </summary>
        public void Dispose()
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

        public string Name { get; } = "BASS";

        public bool CanChangeSourcesDuringRecording => true;
    }
}