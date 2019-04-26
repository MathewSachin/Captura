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

        public override string ToString() => Name;
    }
}