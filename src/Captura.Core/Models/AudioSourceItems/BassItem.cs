using Screna.Audio;

namespace Captura.Models
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
}