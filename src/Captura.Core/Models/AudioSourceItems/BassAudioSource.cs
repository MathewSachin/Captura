using System.Linq;
using Screna.Audio;
using ManagedBass;

namespace Captura.Models
{
    public class BassAudioSource : AudioSource
    {
        class BassItem : NotifyPropertyChanged, IAudioItem
        {
            readonly int _id;

            public BassItem(int Id, string Name)
            {
                _id = Id;
                this.Name = Name;
            }

            public static IAudioProvider GetAudioProvider(BassItem RecordingDevice, BassItem LoopbackDevice)
            {
                return new MixedAudioProvider(RecordingDevice?._id, LoopbackDevice?._id);
            }

            public string Name { get; }

            public override string ToString() => Name;
        }

        static bool AllExist(params string[] Paths)
        {
            return Paths.All(ServiceProvider.FileExists);
        }

        // Check if all BASS dependencies are present
        public static bool Available { get; } = AllExist("ManagedBass.dll", "ManagedBass.Mix.dll", "bass.dll", "bassmix.dll");

        public override IAudioProvider GetAudioProvider()
        {
            var rec = SelectedRecordingSource is BassItem r ? r : null;
            var loop = SelectedLoopbackSource is BassItem l ? l : null;

            if (rec == null && loop == null)
                return null;

            return BassItem.GetAudioProvider(rec, loop);
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