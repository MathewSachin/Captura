using System.Collections.Generic;
using System.Linq;
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

        public static IAudioProvider GetAudioProvider(IEnumerable<BassItem> RecordingDevices, IEnumerable<BassItem> LoopbackDevices)
        {
            var rec = RecordingDevices.Select(M => M._id).ToArray();
            var loop = LoopbackDevices.Select(M => M._id).ToArray();

            return rec.Length + loop.Length == 0 ? null : new MixedAudioProvider(rec, loop);
        }

        public string Name { get; }

        bool _active;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                
                OnPropertyChanged();

                AudioSource.RaiseAudioSourceActiveChanged();
            }
        }

        public override string ToString() => Name;
    }
}