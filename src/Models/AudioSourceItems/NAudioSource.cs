using NAudio.Wave;
using Screna.Audio;
using System.IO;

namespace Captura
{
    public class NAudioSource : AudioSource
    {
        class NAudioItem : IAudioItem
        {
            int _id;
            string _name;

            public NAudioItem(int Id, string Name)
            {
                _id = Id;
                _name = Name;
            }

            public IAudioProvider GetAudioProvider()
            {
                return new WaveInAudioProvider(_id);
            }

            public override string ToString() => _name;
        }

        // Check if NAudio is present
        public static bool Available { get; } = File.Exists("NAudio.dll");

        public override IAudioProvider GetAudioSource()
        {
            if (SelectedRecordingSource is NAudioItem)
                return (SelectedRecordingSource as NAudioItem).GetAudioProvider();

            return null;
        }

        protected override void OnRefresh()
        {
            for (int i = 0; i < WaveIn.DeviceCount; ++i)
                AvailableRecordingSources.Add(new NAudioItem(i, WaveIn.GetCapabilities(i).ProductName));
        }
    }
}