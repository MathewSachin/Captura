using System.Collections.Generic;
using ManagedBass;

namespace Captura.Audio
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

        //static bool AllExist(params string[] Paths)
        //{
        //    return Paths.All(ServiceProvider.FileExists);
        //}

        //// Check if all BASS dependencies are present
        //public static bool Available { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

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

        public IAudioProvider GetAudioProvider(IAudioItem Microphone, IAudioItem Speaker)
        {
            if (Microphone == null && Speaker is BassItem speakerItem)
            {
                return new BassAudioProvider(speakerItem);
            }

            if (Microphone is BassItem micItem && Speaker == null)
            {
                return new BassAudioProvider(micItem);
            }

            if (Microphone is BassItem a && Speaker is BassItem b)
            {
                return new MixedAudioProvider(new[] { a, b });
            }

            return null;
        }

        public string Name { get; } = "BASS";

        public IEnumerable<IAudioItem> Microphones
        {
            get
            {
                for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
                {
                    if (!info.IsLoopback)
                        yield return new BassItem(i, info.Name, info.IsLoopback);
                }
            }
        }

        public IAudioItem DefaultMicrophone => BassDefaultItem.DefaultMicrophone;

        public IEnumerable<IAudioItem> Speakers
        {
            get
            {
                for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
                {
                    if (info.IsLoopback)
                        yield return new BassItem(i, info.Name, info.IsLoopback);
                }
            }
        }

        public IAudioItem DefaultSpeaker => BassDefaultItem.DefaultSpeaker;
    }
}