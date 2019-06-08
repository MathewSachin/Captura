using System.Collections.Generic;
using ManagedBass;

namespace Captura.Audio
{
    /// <summary>
    /// ManagedBass Audio Source.
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
            switch ((Microphone, Speaker))
            {
                case (null, BassItem speaker):
                    return new BassAudioProvider(speaker);

                case (BassItem mic, null):
                    return new BassAudioProvider(mic);

                case (BassItem mic, BassItem speaker):
                    return new MixedAudioProvider(new[] { mic, speaker });

                default:
                    return null;
            }
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