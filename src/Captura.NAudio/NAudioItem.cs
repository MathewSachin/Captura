using Captura.Models;
using NAudio.CoreAudioApi;

namespace Captura.NAudio
{
    class NAudioItem : NotifyPropertyChanged, IAudioItem
    {
        public MMDevice Device { get; }

        public bool IsLoopback { get; }

        public string Name { get; }

        public NAudioItem(MMDevice Device, bool IsLoopback)
        {
            this.Device = Device;
            this.IsLoopback = IsLoopback;

            Name = Device.FriendlyName;
        }

        bool _active;

        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }

        public override string ToString() => Name;
    }
}