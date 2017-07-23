using Screna.Audio;
using ManagedBass;

namespace Captura.Models
{
    public class BassAudioSource : AudioSource
    {
        class BassItem : IAudioItem
        {
            readonly int _id;
            readonly string _name;

            public BassItem(int Id, string Name)
            {
                _id = Id;
                _name = Name;
            }

            public static IAudioProvider GetAudioProvider(BassItem RecordingDevice, BassItem LoopbackDevice)
            {
                return new MixedAudioProvider(RecordingDevice?._id, LoopbackDevice?._id);
            }

            public override string ToString() => _name;
        }

        static bool AllExist(params string[] Paths)
        {
            foreach (var path in Paths)
                if (!ServiceProvider.FileExists(path))
                    return false;

            return true;
        }

        // Check if all BASS dependencies are present
        public static bool Available { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

        public override IAudioProvider GetAudioSource()
        {
            var rec = SelectedRecordingSource is BassItem r ? r : null;
            var loop = SelectedLoopbackSource is BassItem l ? l : null;

            if (rec == null && loop == null)
                return null;

            return BassItem.GetAudioProvider(rec, loop);
        }

        protected override void OnRefresh()
        {
            for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsLoopback)
                    AvailableLoopbackSources.Add(new BassItem(i, info.Name));
                else AvailableRecordingSources.Add(new BassItem(i, info.Name));
            }
        }

        /// <summary>
        /// Initialises BASS and enables Loopback Recording.
        /// Call this method when your application starts.
        /// </summary>
        public override void Init()
        {
            // Initialises Default Playback Device.
            Bass.Init();

            // Enable Loopback Recording.
            Bass.Configure(Configuration.LoopbackRecording, true);
        }

        /// <summary>
        /// Frees all BASS devices.
        /// </summary>
        public override void Dispose()
        {
            for (int i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            {
                if (info.IsInitialized)
                {
                    Bass.CurrentRecordingDevice = i;
                    Bass.RecordFree();
                }
            }

            for (int i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
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